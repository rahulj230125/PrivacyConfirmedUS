pipeline {
    agent any

    parameters {
        string(name: 'BRANCH_NAME', defaultValue: 'master', description: 'Git branch to build')
    }

    environment {
        APP_VM = "10.20.20.20"
        APP_DIR = "/opt/apps/PrivacyConfirmed"
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

		stage('Deploy to App VM') {
			steps {
				sshagent(['app-vm-ssh']) {
					sh """
					ssh -o StrictHostKeyChecking=no appadmin@10.20.20.20 '
						
						cd /opt/apps/PrivacyConfirmed
						git fetch
						git checkout ${params.BRANCH_NAME}
						git reset --hard origin/${params.BRANCH_NAME}

						docker rm -f privacyconfirmed-app || true

						docker build -t privacyconfirmed-app .

						docker run -d -p 5000:8080 \
						  --name privacyconfirmed-app \
						  privacyconfirmed-app
					'
					"""
				}
			}
		}
    }
}