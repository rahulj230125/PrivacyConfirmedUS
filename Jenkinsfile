pipeline {
    agent any

    environment {
        APP_VM = "10.10.10.30"
        APP_USER = "appadmin"

        NEXUS_HOST = "10.20.20.40:5001"
        NEXUS_REPO = "docker-dev"
        IMAGE_NAME = "privacyconfirmed-us-app"

        CONTAINER_NAME = "privacyconfirmed-us-app"
    }

    stages {

        stage('Checkout Code') {
            steps {
                git branch: 'master',
                    credentialsId: 'github-ssh',
                    url: 'git@github.com:rahulj230125/PrivacyConfirmedUS.git'
            }
        }

        stage('Docker Login to Nexus') {
            steps {
                withCredentials([usernamePassword(
                    credentialsId: 'nexus-creds',
                    usernameVariable: 'NEXUS_USER',
                    passwordVariable: 'NEXUS_PASS'
                )]) {

                    sh '''
                    echo "$NEXUS_PASS" | docker login 10.20.20.40:5001 -u "$NEXUS_USER" --password-stdin
                    '''
                }
            }
        }

        stage('Build Docker Image') {
            steps {
                sh '''
                docker build -t ${IMAGE_NAME}:${BUILD_NUMBER} .
                '''
            }
        }

        stage('Tag & Push Image') {
            steps {
                sh '''
                docker tag ${IMAGE_NAME}:${BUILD_NUMBER} ${NEXUS_HOST}/${NEXUS_REPO}/${IMAGE_NAME}:${BUILD_NUMBER}
                docker push ${NEXUS_HOST}/${NEXUS_REPO}/${IMAGE_NAME}:${BUILD_NUMBER}
                '''
            }
        }

        stage('Deploy to App VM') {
            steps {

                withCredentials([
                    usernamePassword(
                        credentialsId: 'nexus-creds',
                        usernameVariable: 'NEXUS_USER',
                        passwordVariable: 'NEXUS_PASS'
                    )
                ]) {

                    sh """
                    ssh -o StrictHostKeyChecking=no ${APP_USER}@${APP_VM} '

                        set -e

                        export VAULT_ADDR=http://vault.pc:8200

                        VAULT_TOKEN=\$(vault write -field=token auth/approle/login \
                            role_id=${ROLE_ID} \
                            secret_id=${SECRET_ID})

                        export VAULT_TOKEN=\$VAULT_TOKEN

                        # Fetch DB values
                        pc_host=\$(vault kv get -field=host kv/privacyconfirmed/db/postgres)
                        pc_database=\$(vault kv get -field=database kv/privacyconfirmed/db/postgres)
                        pc_username=\$(vault kv get -field=iam_username kv/privacyconfirmed/db/postgres)
                        pc_password=\$(vault kv get -field=iam_password kv/privacyconfirmed/db/postgres)

                        echo "DEBUG: pc_host=\$pc_host"
                        echo "DEBUG: pc_database=\$pc_database"

                        # Login to Nexus (safe handling)
                        echo "${NEXUS_PASS}" | docker login ${NEXUS_HOST} -u "${NEXUS_USER}" --password-stdin

                        docker pull ${NEXUS_HOST}/${NEXUS_REPO}/${IMAGE_NAME}:${BUILD_NUMBER}

                        docker rm -f ${CONTAINER_NAME} || true

                        docker run -d -p 6000:8080 \
                          --name ${CONTAINER_NAME} \
                          -e pc_host="\$pc_host" \
                          -e pc_database="\$pc_database" \
                          -e pc_username="\$pc_username" \
                          -e pc_password="\$pc_password" \
                          ${NEXUS_HOST}/${NEXUS_REPO}/${IMAGE_NAME}:${BUILD_NUMBER}

                        echo "DEBUG: Inside container"
                        docker exec ${CONTAINER_NAME} env | grep pc_ || true

                        unset VAULT_TOKEN

                    '
                    """
                }
            }
        }

        stage('Cleanup Docker') {
            steps {
                sh '''
                docker system prune -f || true
                '''
            }
        }
    }

    post {
        success {
            echo "✅ Pipeline completed successfully!"
        }
        failure {
            echo "❌ Pipeline failed!"
        }
    }
}