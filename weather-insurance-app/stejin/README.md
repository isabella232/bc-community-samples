# Weather Insurance

## Prerequisites

- Truffle: npm install -g truffle
- Truffle Flattener: npm install -g truffle-flattener
- Azure Storage Account
- Sql Azure Db

## Instructions

### Compile contracts

cd into Contracts directory and run

```
./build-contracts.sh
```

Upload reference contract files to Azure Storage. 

We want to establish minimum requirements on the interface that must be implemented by users should they wish to create their own contracts. Only contracts that implement the methods included in reference contracts will be accepted. Otherwise, users could attempt to upload random contracts.

Open Azure Storage explorer, expand your account, select "Blob Containers" and create new container "Contracts."

Under "Contracts" create new container "reference."

Add WeatherInsurance.sol from contracts and WeatherInsurance.json from build/contracts to the "reference" blob container.

### Database setup

Create Sql Azure database and copy the connection string.

cd into Services/Integration/Database and run

```
dotnet user-secrets set "ConnectionStrings:Database" "<insert your database connection string>"
dotnet restore
dotnet build
dotnet ef database update
```

cd into Services/Operatior/Functions and run

```
dotnet user-secrets set "ConnectionStrings:Database" "<insert your database connection string>"
dotnet user-secrets set "AzureWebJobsStorage" "<insert your Azure Storage connection string>"
dotnet restore
dotnet build
```

