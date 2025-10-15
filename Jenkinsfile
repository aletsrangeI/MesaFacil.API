pipeline {
  agent any
  environment {
    APP_NAME     = "mesafacil-api"
    RELEASES_DIR = "/opt/mesafacil/api/releases"
    CURRENT_DIR  = "/opt/mesafacil/api/current"
    SHARED_DIR   = "/opt/mesafacil/api/shared"
  }
  options { timestamps(); buildDiscarder(logRotator(numToKeepStr: '20')) }

  stages {
    stage('Checkout') {
      when { branch 'Development' }
      steps { checkout scm }
    }

    stage('Restore & Build') {
      when { branch 'Development' }
      steps {
        sh '''
          set -e
          dotnet restore
          dotnet build -c Release --no-restore
        '''
      }
    }

    stage('Tests') {
      when { branch 'Development' }
      steps {
        sh 'dotnet test -c Release --no-build --logger trx || true'
        junit allowEmptyResults: true, testResults: '**/TestResults/*.trx'
      }
    }

    stage('Publish') {
      when { branch 'Development' }
      steps {
        sh '''
          set -e
          PUBLISH_DIR="$(pwd)/publish"
          rm -rf "$PUBLISH_DIR"
          dotnet publish -c Release -o "$PUBLISH_DIR"
          echo "Publish -> $PUBLISH_DIR"
        '''
      }
    }

    stage('Deploy') {
      when { branch 'Development' }
      steps {
        sh '''
          set -e
          BUILD_ID_SHORT=$(git rev-parse --short HEAD)
          REL_PATH="${RELEASES_DIR}/${BUILD_ID}-${BUILD_ID_SHORT}"

          sudo mkdir -p "$REL_PATH"
          sudo rsync -a --delete "$(pwd)/publish/" "$REL_PATH/"

          # Copiar configs compartidas si las usas:
          # [ -f "${SHARED_DIR}/appsettings.Production.json" ] && sudo cp "${SHARED_DIR}/appsettings.Production.json" "$REL_PATH/"

          # Symlink atómico a current
          sudo ln -sfn "$REL_PATH" "${CURRENT_DIR}"
          sudo chown -h deploy:deploy "${CURRENT_DIR}"
          sudo chown -R deploy:deploy "${RELEASES_DIR}"

          # Reinicia servicio
          sudo systemctl daemon-reload || true
          sudo systemctl restart ${APP_NAME}.service

          # Mantén solo 5 releases
          cd "${RELEASES_DIR}"
          ls -1t | tail -n +6 | xargs -r sudo rm -rf --
        '''
      }
    }
  }

  post { always { cleanWs() } }
}
