#/bin/zsh

apiProject="Plutus.API"
infraProject="Plutus.Infrastructure"
command=$1

if [ -z "$command" ]; then
    echo "Please provide a command: add, remove"
    exit 1
fi

if [ "$command" != "add" ] && [ "$command" != "remove" ]; then
    echo "Invalid command: $command"
    exit 1
fi

if [ "$command" == "add" ]; then
    migraitonName=$2
    if [ -z "$migraitonName" ]; then
        echo "Please provide a migration name"
        exit 1
    fi
    echo "Adding migration..."
    (cd $apiProject && dotnet ef migrations add "$migraitonName" --project ../$infraProject)
    exit 0
fi

if [ "$command" == "remove" ]; then
    echo "Removing migration..."
    (cd $apiProject && dotnet ef migrations remove --project ../$infraProject)
    exit 0
fi
