using System.Collections.Generic;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Purchasing;
using Nethereum.JsonRpc.UnityClient;

[System.Serializable]
public class Item 
{
    public string hero;
    public double price;
    public bool onSale = true;
    public string seller;
    public string productId = "";
    public int heroImageId = -1;
}

[System.Serializable]
public class HeroList
{
    public List<Item> list;
}

public enum Rarity {Common, Uncommon, Rare, Legendary};

public class CardScrollList : MonoBehaviour {

    // List of heroes, objects of Hero class.
    public HeroList heroList;
    // A pool of Hero prefabs, used for optimization purposes.
    public HeroCardPool heroCardPool;
    public Transform contentPanel;

    public ScrollRect scrollView;

	// Use this for initialization.
    void Start () {
        ReloadData();
	}

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                heroList = JsonUtility.FromJson<HeroList>("{\"list\":" + webRequest.downloadHandler.text + "}");
                for (int i = 0; i < heroList.list.Count; i++)
                {
                    heroList.list[i].heroImageId = GetHeroImageId();
                }
                ReloadData();
            }
        }
    }

    private void ReloadData()
    {
        RemoveCards();
        AddCards();
    }

    public static IEnumerator getAccountBalance(string address, System.Action<decimal> callback)
    {
        // Now we define a new EthGetBalanceUnityRequest and send it the testnet url where we are going to
        // check the address, in this case "https://ropsten.infura.io".
        // (we get EthGetBalanceUnityRequest from the Netherum lib imported at the start)
        var getBalanceRequest = new EthGetBalanceUnityRequest("https://ropsten.infura.io/");

        // Then we call the method SendRequest() from the getBalanceRequest we created
        // with the address and the newest created block.
        yield return getBalanceRequest.SendRequest(address, Nethereum.RPC.Eth.DTOs.BlockParameter.CreateLatest());

        // Now we check if the request has an exception
        if (getBalanceRequest.Exception == null)
        {
            // We define balance and assign the value that the getBalanceRequest gave us.
            var balance = getBalanceRequest.Result.Value;
            // Finally we execute the callback and we use the Netherum.Util.UnitConversion
            // to convert the balance from WEI to ETHER (that has 18 decimal places)
            callback(Nethereum.Util.UnitConversion.Convert.FromWei(balance, 18));
        }
        else
        {
            // If there was an error we just throw an exception.
            throw new System.InvalidOperationException("Get balance request failed");
        }

    }

    public void LoadHeroes() {
        RectTransform scrollViewTransform = scrollView.GetComponent<RectTransform>();
        scrollViewTransform.offsetMax = new Vector2(scrollViewTransform.offsetMax.x, -500);
        heroList.list = new List<Item>();
        ReloadData();

        // TODO get heroes for current player.
        getAccountBalance();
    }

    public void LoadMarket()
    {
        RectTransform scrollViewTransform = scrollView.GetComponent<RectTransform>();
        scrollViewTransform.offsetMax = new Vector2(scrollViewTransform.offsetMax.x, -100);
        heroList.list = new List<Item>();
        ReloadData();
        StartCoroutine(GetRequest("https://heromarket.azurewebsites.net/api/Offers"));
    }

    public void LoadStore()
    {
        RectTransform scrollViewTransform = scrollView.GetComponent<RectTransform>();
        scrollViewTransform.offsetMax = new Vector2(scrollViewTransform.offsetMax.x, -100);

        heroList.list = new List<Item>();

        Item pack = new Item();
        pack.hero = "Random Bronze Hero";
        pack.seller = "Cryptic Legends";
        pack.price = 0.99;
        pack.productId = ProductID.NonConsumableBronze;
        pack.heroImageId = -1;
        heroList.list.Add(pack);

        Item packS = new Item();
        packS.hero = "Random Silver Hero";
        packS.seller = "Cryptic Legends";
        packS.price = 1.99;
        packS.productId = ProductID.NonConsumableSilver;
        packS.heroImageId = -2;
        heroList.list.Add(packS);

        Item packG = new Item();
        packG.hero = "Random Gold Hero";
        packG.seller = "Cryptic Legends";
        packG.price = 6.99;
        packG.productId = ProductID.NonConsumableGold;
        packG.heroImageId = -3;
        heroList.list.Add(packG);


        Item packL = new Item();
        packL.hero = "Random Legendary Hero";
        packL.seller = "Cryptic Legends";
        packL.price = 17.99;
        packL.productId = ProductID.NonConsumableLegendary;
        packL.heroImageId = -4;
        heroList.list.Add(packL);

        ReloadData();
    }

    private void RemoveCards() {
        while (contentPanel.childCount > 0)
        {
            GameObject toRemove = transform.GetChild(0).gameObject;
            heroCardPool.ReturnObject(toRemove);
        }
    }

    private static int currentImageId = -1;
    private int GetHeroImageId()
    {
        if (currentImageId > 19)
        {
            currentImageId = -1;
        }
        currentImageId++;
        return currentImageId;
    }

    private void AddCards () {
        for (int i = 0; i < heroList.list.Count; i++) {
            Item hero = heroList.list[i];
            GameObject newHeroCard = heroCardPool.GetObject();
            newHeroCard.transform.SetParent(contentPanel);

            HeroCard heroCard = newHeroCard.GetComponent<HeroCard>();
            heroCard.Setup(hero, this);
        }
    }
}
