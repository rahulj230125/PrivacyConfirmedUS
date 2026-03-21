pipeline {
    agent any

    parameters {
        string(name: 'BRANCH_NAME', defaultValue: 'main', description: 'Git branch to build')
    }

    environment {
        IMAGE_NAME = "privacyconfirmed-app"
        NEXUS_DEV = "nexus.pc:5001/docker-dev"
        APP_VM = "10.20.20.20"
        APP_USER = "appadmin"
        CONTAINER_NAME = "privacyconfirmed-app"
    }

    stages {

        stage('Checkout Code') {
            steps {
                git branch: "${params.BRANCH_NAME}",
                    url: 'git@github.com:rahulj230125/PrivacyConfirmed.git',
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
                    sh """
                    echo "$NEXUS_PASS" | docker login nexus.pc:5001 -u "$NEXUS_USER" --password-stdin
                    """
                }
            }
        }

        stage('Build Docker Image') {
            steps {
                sh """
                docker build -t ${IMAGE_NAME}:${BUILD_NUMBER} .
                """
            }
        }

        stage('Tag & Push Image to Nexus') {
            steps {
                sh """
                docker tag ${IMAGE_NAME}:${BUILD_NUMBER} ${NEXUS_DEV}/${IMAGE_NAME}:${BUILD_NUMBER}
                docker push ${NEXUS_DEV}/${IMAGE_NAME}:${BUILD_NUMBER}
                """
            }
        }

		stage('Deploy to App VM') {
			steps {
				withCredentials([usernamePassword(
					credentialsId: 'nexus-creds',
					usernameVariable: 'NEXUS_USER',
					passwordVariable: 'NEXUS_PASS'
				)]) {
					sshagent(['app-vm-ssh']) {
						sh """
						ssh -o StrictHostKeyChecking=no ${APP_USER}@${APP_VM} '
							
							echo "${NEXUS_PASS}" | docker login 10.20.20.40:5001 -u "${NEXUS_USER}" --password-stdin

							docker pull ${NEXUS_DEV}/${IMAGE_NAME}:${BUILD_NUMBER}

							docker rm -f ${CONTAINER_NAME} || true

							docker run -d -p 5000:8080 \
							  --name ${CONTAINER_NAME} \
							  ${NEXUS_DEV}/${IMAGE_NAME}:${BUILD_NUMBER}
						'
						"""
					}
				}
			}
		}
		stage('Cleanup Docker') {
			steps {
				sh '''
				docker system prune -f
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