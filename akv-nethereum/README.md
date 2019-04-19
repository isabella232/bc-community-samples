# Azure Key Vault And Azure Function (Java)

## Table of Contents

* [Table of Contents](#table-of-contents)
* [Overview](#overview)
* [Prerequisites](#prerequisites)
* [Step 1 - Login to Azure](#step-1---login-to-azure)
* [Step 2 - Generate Ethereum Keypair](#step-2---generate-ethereum-keypair)
* [Step 3 - Deploy Ethereum PoA Network on Azure Cloud](#step-3---deploy-ethereum-poa-network-on-azure-cloud)
* [Step 4 - Create an Azure Key Vault](#step-4---create-an-azure-key-vault)
* [Step 5 - Store Ethereum Secrets on Azure Key Vault](#step-5---store-ethereum-secrets-on-azure-key-vault)
* [Step 6 - List Stored Secrets on Azure Key Vault](#step-6---list-stored-secrets-on-azure-key-vault)
* [Step 7 - Clone the Repo](#step-7---clone-the-repo)
* [Step 8 - Configure Application Settings](#step-8---configure-application-settings)
* [Step 9 - Test the Azure Function Locally](#step-9---test-the-azure-function-locally)
* [Step 10 - Deploy the Azure Function to Azure Cloud](#step-10---deploy-the-azure-function-to-azure-cloud)
* [Troubleshoot](#troubleshoot)

## Overview

By following this solution, you will learn how to:

 - Generate an Ethereum keypair locally using Go Ethereum.
 - Deploy an Ethereum PoA network on Azure Cloud.
 - Store and retrieve Ethereum secrets using Azure Key Vault Secrets API.
 - Sign and send Ethereum transactions using Azure Function in .NET

## Prerequisites

* POSIX compliant command line
* [Go Ethereum](https://geth.ethereum.org/)
* [.NET Core 2.x](https://docs.microsoft.com/en-us/dotnet/core/tools/?tabs=netcore2x)
* [Git](https://www.git-scm.com/)
* [Curl](https://curl.haxx.se/download.html)
* [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli?view=azure-cli-latest) 2.0.4 or later
* [Azure Function Core Tools](https://github.com/Azure/azure-functions-core-tools)
* An Azure subscription. If you don't have an Azure subscription, create a [free account](https://azure.microsoft.com/free/?WT.mc_id=A261C142F) before you begin.

## Step 1 - Login to Azure

To log in to Azure using the CLI, run:

```bash
az login
```

## Step 2 - Generate Ethereum Keypair

To generate an Ethereum keypair for development purpose, run:

```bash
geth account new
```

Please make sure to remember the passphrase you entered for the new keypair.  

## Step 3 - Deploy Ethereum PoA Network on Azure Cloud

Follow [this guide](./EthereumPoA.md) to deploy an Ethereum PoA Network on Azure Cloud.

When asked for entering an `Admin Ethereum Address` during the deployment, fill in the Ethereum address created in the previous step. 

## Step 4 - Create an Azure Key Vault

Next you create a Key Vault using the resource group created in the previous step. Please note you have to use a globally unique name. Provide the following information:

* `<YourKeyVaultName>` - **Select an Unique Key Vault Name here**.
* `<YourResourceGroupName>` - **Use the same resource group with the deployed Ethereum PoA Network in the previous step**.

```bash
export YourKeyVaultName=<YourKeyVaultName>
export YourResourceGroupName=<YourResourceGroupName>
az keyvault create --name "$YourKeyVaultName" --resource-group "$YourResourceGroupName" --location "westus"
```

At this point, your Azure account is the only one authorized to perform any operations on this new vault.

## Step 5 - Store Ethereum Secrets on Azure Key Vault

First, list local key stores and locate the keystore file for development purpose:
```bash
geth account list
```

Second, store the keystore JSON file:
```bash
az keyvault secret set --vault-name "$YourKeyVaultName" --name "EthKeystore" --file <PathToYourKeyStoreFile>
```

Third, store the passphrase of the keystore:
```bash
az keyvault secret set --vault-name "$YourKeyVaultName" --name "EthKeystorePassphrase" --value "<YourPassphrase>"
```

## Step 6 - List Stored Secrets on Azure Key Vault

To see the stored Ethereum secrets on Azure Key Vault, run:

```bash
az keyvault secret list --vault-name "$YourKeyVaultName"
``` 

Please make sure the output lists 2 secrets including `EthKeystore` and `EthKeystorePassphrase` created in the previous step. 

## Step 7 - Clone the Repo

Clone the repo in order to make a local copy for you to edit the source by running the following command:

```
git clone https://github.com/Azure-Samples/bc-community-samples.git
cd akv-nethereum
```

## Step 8 - Configure Application Settings 

First, copy [local.settings.sample.json](./local.settings.sample.json) to [local.settings.json](local.settings.json) by running:

```bash
cp local.settings.sample.json local.settings.json
```

Second, open [local.settings.json](./local.settings.json) and change the following values with the results from the Azure CLI commands:

**ETH_JSON_RPC**:

```bash
echo "http://$(az network public-ip show --ids $(az network lb frontend-ip show --resource-group "$YourResourceGroupName" --lb-name $(az network lb list --query [0].name | tr -d '"') --name LBFrontEnd --query publicIpAddress.id | tr -d '"') --query ipAddress | tr -d '"'):8540"
```

**VAULT_ID**: 

```bash
az keyvault show --name "$YourKeyVaultName" --query id
```

**KEYSTORE_SECRET_ID**: 

```bash
az keyvault secret show --vault-name "$YourKeyVaultName" --name EthKeystore --query id
```

**PASSPHRASE_SECRET_ID**: 

```bash
az keyvault secret show --vault-name $YourKeyVaultName --name EthKeystorePassphrase --query id
```

## Step 9 - Test the Azure Function Locally

First, compile and run the Azure Function locally:

```bash
func host start --build
```

Second, call the Azure Function to sign and send an Ethereum transaction to the deployed PoA network:

```bash
curl http://localhost:7071/api/send_test_transaction
```

An Ethereum transaction receipt will be returned after a few seconds if the transaction is successfully sent to the PoA network. 

## Step 10 - Deploy the Azure Function to Azure Cloud

Create a storage account for storing the artifacts of Azure Functions:
```
export YourFunctionName=akv-nethereum
export YourStorageAccountName=akvnethereum
az storage account create --name $YourStorageAccountName --location westus \
    --resource-group $YourResourceGroupName --sku Standard_LRS
```

Create a function app with `dotnet` runtime:
```
az functionapp create --resource-group $YourResourceGroupName --consumption-plan-location westus \
    --name $YourFunctionName --storage-account $YourStorageAccountName --runtime dotnet 
```

After the function app is created, run the following commands to assign readonly permissions of Azure Key Vault to the deployed function app: 
```bash
az functionapp identity assign --name $YourFunctionName --resource-group $YourResourceGroupName
export YourFunctionObjectId=$(az functionapp identity show --resource-group $YourResourceGroupName --name $YourFunctionName --query principalId | tr -d '"')
az keyvault set-policy --name $YourKeyVaultName --object-id $YourFunctionObjectId --secret-permissions get
```

Last, publish the function with local settings:
```
func azure functionapp publish $YourFunctionName --publish-local-settings --overwrite-settings
```

To test the deployed Azure Function, run:
```bash
curl https://$YourFunctionName.azurewebsites.net/api/send_test_transaction
```

## Troubleshoot

- If you receive 404 error when testing the deployed function on Azure Cloud, try to run `func azure functionapp publish ` a few more times.