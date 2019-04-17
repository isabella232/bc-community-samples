# Azure Key Vault And Nethereum

The project contains Http Triggered function `SendTx`
It does:
    * Retrieves private key from KeyVault based on key identifier
    * Creates Ethereum account based on the private key
    * Sign transaction by account's private key
    * Send the transaction to a node via RPC connection
    * Returns response of the execution

## Getting started
1. Create Azure FunctionApp
2. Create KeyVault
3. Configure identity of Azure FunctionApp
4. Set policy for the FunctionApp in KeyVault
5. Deploy FunctionApp
6. Configure FunctionApp

## Configuration

In order to run the function, you need to provide configuration via `local.settings.json` or you add configuration to Azure FunctionApp.

local.settings.json:
```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "AzureWebJobsDashboard": "UseDevelopmentStorage=true",
    "Ethereum:RPC": "http://127.0.0.1:8545",
    "Ethereum:GasLimit": "6721975"
  }
}
```

## AuthorizationLevel

The function has `Function` protection level.

In order to run the function on FunctionApp, you need to add key to url, like `https://func-neth-app.azurewebsites.net/api/sendTx?code=HqssNO/Do4OGncDxa22i....==`

In order to run it locally, you dont need anything extra. `http://localhost:7071/api/sendTx`

## Key Vault

Before execution of the function you ned to create `Secret` in the KeyVault.
The key should contain private key of Ethereum account. Then you need to retrieve the KeyIdentifier.

## Execution

In order to execute the function you can use Postman
You need to create POST request with json body:

```
{
	"identifier": "https://funcnethappkeyvault.vault.azure.net/secrets/0a3ccf72-2883-4138-a5ce-a56e5057973d/e9b2ff27897b4fef92fa7bd5b12214fc",
	"tx": {
		"value": "0",
		"to": "0xF4485b57bE9ACad0253815b7000A7C1fE1D7EEFC",
		"gas": "0x5208",
		"gasPrice": "0x4A817C800",
		"data": "0x"
	}
}
```

The data contains two main parts:
1. identifier - KeyVault identifier, which used for retrieving private key
2. tx - transaction data. All properties of the field should hex format
