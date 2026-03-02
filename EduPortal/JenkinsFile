pipeline {
    agent {
        docker { 
            image 'mcr.microsoft.com/dotnet/sdk:8.0' 
        }
    }

    stages {
        stage('Checkout') {
            steps {
                echo 'Pull repository...'
                checkout scm
            }
        }

        stage('Restore & Build') {
            steps {
                echo 'Build solution...'
                sh 'dotnet restore'
                sh 'dotnet build --configuration Release'
            }
        }

        stage('Run Tests') {
            steps {
                echo 'Test starts...'
                sh 'dotnet test --no-build --configuration Release'
            }
        }
    }

    post {
        always {
            echo 'Pipeline finished.'
        }
    }
}