#!/bin/bash
# =========================
# Script SonarQube Analysis
# =========================

# Fonction pour gérer les couleurs dans l'affichage
colors() {
    RED="\033[0;31m"
    GREEN="\033[0;32m"
    YELLOW="\033[1;33m"
    CYAN="\033[1;36m"
    NC="\033[0m" # Réinitialisation
    printf "${!1}${2} ${NC}\n"
}

# --------------------
# 1. Vérification des Pré-requis
# --------------------

# Vérification de la présence du fichier .env
if [ ! -f .env ]; then
    colors "RED" "Erreur : Fichier .env non trouvé. Veuillez configurer les variables nécessaires."
    exit 1
fi
source .env

# Détection automatique de la solution .sln
SONAR_PROJECT_KEY=$(ls *.sln | sed -E 's/\.sln$//')
SOLUTION_FILE=$(ls *.sln)

# Vérification des variables essentielles
required_vars=("SONAR_PROJECT_KEY" "SONAR_HOST_URL" "SONAR_USER_TOKEN" "COVERAGE_REPORT_PATH" "SOLUTION_FILE" "BUILD_CONFIGURATION")
for var in "${required_vars[@]}"; do
    if [[ -z "${!var}" ]]; then
        colors "RED" "La variable $var n'est pas définie. Veuillez vérifier votre configuration."
        exit 1
    fi
done

# --------------------
# 2. Vérification du Serveur SonarQube
# --------------------
colors "CYAN" "Vérification de l'état du serveur SonarQube à l'adresse $SONAR_HOST_URL"
check_server=$(curl -s -L -o /dev/null -w "%{http_code}" "$SONAR_HOST_URL")

if [[ "$check_server" != "200" && "$check_server" != "302" ]]; then
    colors "RED" "Erreur : Le serveur SonarQube est inaccessible. Code HTTP: $check_server"
    exit 1
fi

# --------------------
# 3. Analyse SonarQube
# --------------------
colors "YELLOW" "Démarrage de l'analyse SonarQube pour le projet $SONAR_PROJECT_KEY"

# Vérification de la présence de dotnet-sonarscanner
if ! command -v dotnet-sonarscanner &>/dev/null; then
    colors "CYAN" "SonarScanner pour .NET non trouvé. Installation en cours..."
    # Installation de dotnet-sonarscanner
    if dotnet tool install --global dotnet-sonarscanner --version 6.0.0; then
        colors "GREEN" "SonarScanner pour .NET installé avec succès."
        export PATH="$PATH:$HOME/.dotnet/tools"
    else
        colors "RED" "Erreur : Impossible d'installer dotnet-sonarscanner."
        exit 1
    fi
else
    colors "CYAN" "SonarScanner pour .NET déjà installé."
fi

# Vérification et installation de Coverlet
if ! command -v coverlet &>/dev/null; then
    colors "CYAN" "Coverlet non trouvé. Installation en cours..."
    # Installation de Coverlet
    if dotnet tool install --global coverlet.console; then
        colors "GREEN" "Coverlet installé avec succès."
    else
        colors "RED" "Erreur : Impossible d'installer Coverlet."
        exit 1
    fi
else
    colors "CYAN" "Coverlet déjà installé."
fi

# Lancement de l'analyse SonarQube
dotnet sonarscanner begin \
    /k:"$SONAR_PROJECT_KEY" \
    /d:sonar.host.url="$SONAR_HOST_URL" \
    /d:sonar.token="$SONAR_USER_TOKEN" \
    /d:sonar.cs.opencover.reportsPaths="$(pwd)/coverage.opencover.xml"

# --------------------
# 4. Restauration du projet
# --------------------
dotnet restore "$SOLUTION_FILE"
colors "YELLOW" "Restauration du projet terminée"

# --------------------
# 5. Compilation du Projet
# --------------------
colors "YELLOW" "Compilation de la solution $SOLUTION_FILE avec la configuration $BUILD_CONFIGURATION"
dotnet build "$SOLUTION_FILE" --configuration "$BUILD_CONFIGURATION" --no-restore

# Vérification de la réussite de la compilation
if [[ $? -ne 0 ]]; then
    colors "RED" "Échec de la compilation. Analyse SonarQube interrompue."
    exit 1
fi

# --------------------
# 6. Exécution des tests et couverture de code
# --------------------
colors "YELLOW" "Exécution des tests et génération de la couverture de code avec Coverlet"

# Utilisation de CI_PROJECT_DIR pour garantir le chemin correct
if [ ! -f "$CI_PROJECT_DIR/${SONAR_PROJECT_KEY}.Tests/bin/Release/net6.0/${SONAR_PROJECT_KEY}.Tests.dll" ] && [ ! -f "$CI_PROJECT_DIR/${SONAR_PROJECT_KEY}.Tests/bin/Debug/net6.0/${SONAR_PROJECT_KEY}.Tests.dll" ]; then
    colors "RED" "Le fichier DLL de tests n'existe ni dans le répertoire Release ni dans Debug."
    exit 1
else
    # Utilisation du chemin correct en fonction de l'existence du fichier
    if [ -f "$CI_PROJECT_DIR/${SONAR_PROJECT_KEY}.Tests/bin/Release/net6.0/${SONAR_PROJECT_KEY}.Tests.dll" ]; then
        TEST_DLL_PATH="$CI_PROJECT_DIR/${SONAR_PROJECT_KEY}.Tests/bin/Release/net6.0/${SONAR_PROJECT_KEY}.Tests.dll"
    else
        TEST_DLL_PATH="$CI_PROJECT_DIR/${SONAR_PROJECT_KEY}.Tests/bin/Debug/net6.0/${SONAR_PROJECT_KEY}.Tests.dll"
    fi
fi

colors "CYAN" "Chemin vers les tests : $TEST_DLL_PATH"

# Exécution des tests avec Coverlet
coverlet "$TEST_DLL_PATH" \
    --target "dotnet" \
    --targetargs "test --no-build" \
    -f=opencover \
    -o="$(pwd)/coverage.opencover.xml" \
    2>&1 | tee coverlet_output.log

# --------------------
# 7. Vérification de la création du fichier de couverture
# --------------------
colors "CYAN" "Vérification des résultats de couverture dans le fichier coverage.opencover.xml"
if grep -q "<line coverage=\"0\"/>" "$(pwd)/coverage.opencover.xml"; then
    colors "RED" "Aucune couverture n'a été générée, les tests ne semblent pas couvrir le code."
    exit 1
else
    colors "GREEN" "La couverture semble avoir été générée correctement."
fi

# --------------------
# 8. Fin de l'analyse SonarQube
# --------------------
colors "GREEN" "Analyse SonarQube terminée avec succès."
dotnet sonarscanner end /d:sonar.token="$SONAR_USER_TOKEN"

if [[ $? -ne 0 ]]; then
    colors "RED" "Échec de l'analyse SonarQube.."
    exit 1
fi

# --------------------
# 9. Message de Succès
# --------------------
colors "GREEN" "####################### Analyse SonarQube terminée avec succès ##########################"
colors "CYAN"  "|  Rapport de couverture généré et envoyé à SonarQube                                   |"
colors "CYAN"  "|  Serveur SonarQube accessible et analyse effectuée                                    |"
colors "GREEN" "#########################################################################################"
exit 0