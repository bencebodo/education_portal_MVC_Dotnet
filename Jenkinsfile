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
                dir('EduPortal') {
                    echo 'Build solution...'
                    sh 'dotnet restore'
                    sh 'dotnet build --configuration Release'
                }
            }
        }
        stage('Unit tests') {
            steps {
                script{
                updateGitHubStatus('ci/jenkins/unit-tests', 'Unit tests started...', 'PENDING')
                }
                catchError(buildResult: 'FAILURE', stageResult: 'FAILURE') {
                    sh 'dotnet test EduPortal.UnitTests/EduPortal.UnitTests.csproj'
                }
        
                script {
                    if (currentBuild.result == 'FAILURE') {
                        updateGitHubStatus('ci/jenkins/unit-tests', 'Unit tests failed!', 'FAILURE')
                    } else {
                        updateGitHubStatus('ci/jenkins/unit-tests', 'Unit tests passed!', 'SUCCESS')
                    }
                }
            }
        }
    }

    post {
        always {
            echo 'Pipeline finished.'
        }
    }
}

def updateGitHubStatus(String contextName, String msg, String state) {
    step([
        $class: 'GitHubCommitStatusSetter',
        contextSource: [
            $class: 'StaticContextSource', 
            context: contextName
        ],
        statusResultSource: [
            $class: 'ConditionalStatusResultSource', 
            results: [
                [
                    $class: 'AnyBuildResult', 
                    message: msg, 
                    state: state
                ]
            ]
        ]
    ])
}