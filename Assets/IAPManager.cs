using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

public class IAPManager : MonoBehaviour
{

    private string removeAds = "com.bhSoftware.dopecalculator&.removeads";
    public GameObject restoreButton;

    private void Awake()
    {
        if (Application.platform != RuntimePlatform.IPhonePlayer)
        {
            restoreButton.SetActive(false);
        }
            
    }

    public void OnPurchaseComplete(Product product)
    {
        if (product.definition.id == removeAds)
        {
            Debug.Log("All ads removed");
        }
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log(product.definition.id + " failed because " + failureReason);
    }
} 