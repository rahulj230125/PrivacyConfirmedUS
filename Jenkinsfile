pipeline {
    agent any

    parameters {
        string(name: 'BRANCH_NAME', defaultValue: 'main', description: 'Git branch to build')
    }

    environment {
        IMAGE_NAME = "privacyconfirmed-us-app"
        NEXUS_HOST = "10.20.20.40:5001"
        NEXUS_REPO = "docker-dev"
        APP_VM = "10.10.10.30"
        APP_USER = "appadmin"
        CONTAINER_NAME = "privacyconfirmed-us-app"
    }

    stages {

        stage('Checkout Code') {
            steps {
                git branch: "${params.BRANCH_NAME}",
                    url: 'git@github.com:rahulj230125/PrivacyConfirmedUS.git',
                    credentialsId: 'jenkins-github-privacyconfirmed-repo'
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
                    echo "$NEXUS_PASS" | docker login ${NEXUS_HOST} -u "$NEXUS_USER" --password-stdin
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

        stage('Tag & Push Image to Nexus') {
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
                    ),
                    string(credentialsId: 'vault-role-id', variable: 'ROLE_ID'),
                    string(credentialsId: 'vault-secret-id', variable: 'SECRET_ID')
                ]) {

                    sshagent(['app-vm-ssh']) {

                        sh '''
                        ssh -o StrictHostKeyChecking=no ${APP_USER}@${APP_VM} "

                            export VAULT_ADDR=http://vault.pc:8200

                            VAULT_TOKEN=\$(vault write -field=token auth/approle/login \
                                role_id=${ROLE_ID} \
                                secret_id=${SECRET_ID})

                            export VAULT_TOKEN=\$VAULT_TOKEN

                            # 🔥 FETCH VALUES FROM VAULT
                            pc_host=\$(vault kv get -field=pc_host kv/privacyconfirmed/db/postgres)
                            pc_database=\$(vault kv get -field=pc_database kv/privacyconfirmed/db/postgres)
                            pc_username=\$(vault kv get -field=pc_username kv/privacyconfirmed/db/postgres)
                            pc_password=\$(vault kv get -field=pc_password kv/privacyconfirmed/db/postgres)

                            # 🔥 EXPORT FOR DOCKER
                            export pc_host=\$pc_host
                            export pc_database=\$pc_database
                            export pc_username=\$pc_username
                            export pc_password=\$pc_password

                            echo \"$NEXUS_PASS\" | docker login ${NEXUS_HOST} -u \"$NEXUS_USER\" --password-stdin

                            docker pull ${NEXUS_HOST}/${NEXUS_REPO}/${IMAGE_NAME}:${BUILD_NUMBER}

                            docker rm -f ${CONTAINER_NAME} || true

                            docker run -d -p 6000:8080 \\
                              --name ${CONTAINER_NAME} \\
                              -e pc_host=\$pc_host \\
                              -e pc_database=\$pc_database \\
                              -e pc_username=\$pc_username \\
                              -e pc_password=\$pc_password \\
                              ${NEXUS_HOST}/${NEXUS_REPO}/${IMAGE_NAME}:${BUILD_NUMBER}
                        "
                        '''
                    }
                }
            }
        }

        stage('Cleanup Docker') {
            steps {
                sh '''
                docker image prune -f
                '''
            }
        }
    }

    post {
        success {
            echo "✅ Deployment successful!"
        }
        failure {
            echo "❌ Pipeline failed!"
        }
    }
}