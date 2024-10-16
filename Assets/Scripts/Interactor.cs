using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [SerializeField] ItemId handItem;
    ItemId groundItem;
    TileObject tileObject;
    ItemObject itemObject;
    [SerializeField] int depth;
    GroundMaterial groundMaterial;
    Interactable interactable;

    [SerializeField] Transform cam;
    [SerializeField] Transform handTransform;

    [SerializeField] Transform marker;

    [SerializeField] LayerMask tileMask;

    [SerializeField] TileManager firmTileManager;

    [SerializeField] ItemMapping itemMap;
    [SerializeField] GroundMaterialMapping groundMaterialMap;
    [SerializeField] RecipeMapping recipeMap;

    [SerializeField] Action pullAction;
    [SerializeField] Action pushAction;
    Recipe pullRecipe;
    Recipe pushRecipe;

    [SerializeField] GameObject leftIndicator;
    [SerializeField] GameObject rightIndicator;

    int targetYRotation;

    private void Update()
    {
        FindGroundWithRay();
        if (tileObject != null)
        {

            if (pushRecipe != null && pushRecipe.keepRotation)
            {
                targetYRotation = firmTileManager.tileDatas[tileObject.id].GetItem(depth).rotation;
            }
            else
            {
                targetYRotation = FindPlayerYRotation();
            }

            UpdateMarkerRotation(targetYRotation);
        }
    }

    public void Pull(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            HandleAction(pullAction, pullRecipe);
            FindActions();
        }
    }
    public void Push(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            HandleAction(pushAction, pushRecipe);
            FindActions();
        }
    }
    void HandleAction(Action action, Recipe recipe)
    {
        switch (action)
        {
            case Action.None:
                return;
            case Action.Grab:
                Debug.Log("pick up object");
                SpawnHandItem(groundItem);
                DespawnGroundItem();
                return;
            case Action.Drop:
                Debug.Log("drop object");
                SpawnGroundItem(handItem, targetYRotation);
                DespawnHandItem();
                return;
            case Action.Craft:
                Debug.Log("craft object");
                if (recipe.inputHand != recipe.outputHand)
                {
                    DespawnHandItem();
                    SpawnHandItem(recipe.outputHand);
                }
                if (recipe.inputGround != recipe.outputGround[0]) // Fikse her så ting oppå blir spawna selv om første er lik
                {
                    int rotation = firmTileManager.tileDatas[tileObject.id].GetItem(depth).rotation;
                    DespawnGroundItem();
                    for (int i = 0; i < recipe.outputGround.Length; i++)
                    {
                        if (recipe.keepRotation)
                        {
                            SpawnGroundItem(recipe.outputGround[i], rotation);
                        }
                        else
                        {
                            SpawnGroundItem(recipe.outputGround[i], targetYRotation);
                        }
                    }
                }
                return;
            case Action.Dig:
                LowerTerrain();
                SpawnGroundItem(groundMaterialMap.GetGroundMaterialDefinition(groundMaterial).item, targetYRotation);
                return;
            case Action.Fill:
                RaiseTerrain(groundMaterialMap.GetGroundMaterialDefinition(groundItem).groundMaterial);
                DespawnGroundItem();
                return;
            case Action.Interact:
                if (handItem != interactable.outputHand)
                {
                    DespawnHandItem();
                    SpawnHandItem(interactable.outputHand);
                }
                if (groundItem != interactable.outputGround)
                {
                    DespawnGroundItem();
                    SpawnGroundItem(interactable.outputGround, targetYRotation);
                }
                interactable.Interact();
                return;
        }
    }

    void SpawnHandItem(ItemId item)
    {
        handItem = item;
        if (item != ItemId.None)
        {
            Instantiate(itemMap.GetItemDefinition(item).prefab, handTransform);
        }
    }
    void DespawnHandItem()
    {
        handItem = ItemId.None;
        if (handTransform.childCount > 0)
        {
            Destroy(handTransform.GetChild(0).gameObject);
        }
    }
    void SpawnGroundItem(ItemId item, int rotation)
    {
        //firmTileManager.tileDatas[tileObject.id].itemSlot = new ItemData(item, rotation);
        firmTileManager.tileDatas[tileObject.id].AddItem(depth, new ItemData(item, rotation));

        if (itemObject == null)
        {
            itemObject = tileObject.SpawnItem(itemMap.GetItemDefinition(item).prefab, rotation);
        }
        else
        {
            itemObject = itemObject.SpawnItem(itemMap.GetItemDefinition(item).prefab, rotation);
        }

        groundItem = item;
    }
    void DespawnGroundItem()
    {
        firmTileManager.tileDatas[tileObject.id].RemoveItem(depth);
        //tileObject.DespawnItem();
        //itemObject = null;
        if (depth == 0)
        {
            //firmTileManager.tileDatas[tileObject.id].itemSlot = null;
            tileObject.DespawnItem();
            itemObject = null;
        }
        else
        {
            itemObject = itemObject.transform.parent.GetComponent<ItemObject>();
            itemObject.DespawnItem();
        }
    }
    void RaiseTerrain(GroundMaterial groundMaterial)
    {
        StartCoroutine(firmTileManager.UpdateTile(tileObject.id, tileObject, -0.25f, groundMaterial));
        //Debug.Log("Raise Terrain");
    }
    void LowerTerrain()
    {
        StartCoroutine(firmTileManager.UpdateTile(tileObject.id, tileObject, 0.25f));
        //Debug.Log("Lower Terrain");
    }

    int FindPlayerYRotation()
    {
        //Vector3 playerPos = tileObject.transform.InverseTransformPoint(transform.position);

        //float localYAngle = Mathf.Atan2(playerPos.x, playerPos.z) * Mathf.Rad2Deg;
        //float localYAngle = tileObject.transform.InverseTransformVector(transform.eulerAngles).y;

        //return Mathf.RoundToInt(localYAngle);

        Vector3 localPlayerForward;

        localPlayerForward = tileObject.transform.InverseTransformDirection(transform.forward);

        /*
        if (itemObject != null)
        {
            localPlayerForward = itemObject.transform.InverseTransformDirection(transform.forward);
        }
        else
        {
            localPlayerForward = tileObject.transform.InverseTransformDirection(transform.forward);
        }*/

        float localYAngle = Mathf.Atan2(localPlayerForward.x, localPlayerForward.z) * Mathf.Rad2Deg;

        return Mathf.RoundToInt(localYAngle);
    }

    void FindActions() // kan alltid bli gjort lokalt for å gi tooltips, men blir sjekka med server når handling faktisk blir gjort?
    {
        pullAction = Action.None;
        pushAction = Action.None;

        if (tileObject == null)
        {
            leftIndicator.SetActive(false);
            rightIndicator.SetActive(false);
            return;
        }

        if (firmTileManager.tileDatas[tileObject.id].GetItem(depth) == null)
        {
            groundItem = ItemId.None;
            groundMaterial = firmTileManager.tileDatas[tileObject.id].groundMaterial[firmTileManager.tileDatas[tileObject.id].groundMaterial.Count-1];

            if (handItem != ItemId.None)
            {
                pushAction = Action.Drop;
            }

            if (groundMaterialMap.IsDiggableBy(groundMaterial, handItem))
            {
                pullAction = Action.Dig;
            }
        }
        else
        {
            groundItem = firmTileManager.tileDatas[tileObject.id].GetItem(depth).item;

            if (handItem == ItemId.None)
            {
                if (itemMap.GetItemDefinition(groundItem).pickupable)
                {
                    pullAction = Action.Grab;
                }
            }

            if (groundMaterialMap.IsFillableBy(groundItem, handItem))
            {
                pushAction = Action.Fill;
            }

            if (tileObject.GetComponentInChildren<Interactable>())
            {
                interactable = tileObject.GetComponentInChildren<Interactable>();
                if (interactable.enabled == true)
                {
                    if (interactable.inputHand == handItem)
                    {
                        if (interactable.pull == true)
                        {
                            pullAction = Action.Interact;
                        }
                        else
                        {
                            pushAction = Action.Interact;
                        }
                    }
                }
            }
        }

        //Check crafting
        Recipe recipe = recipeMap.GetRecipe(handItem, true, groundItem); // legge til å se om de er blocking
        if (recipe != null)
        {
            pullAction = Action.Craft;
            pullRecipe = recipe;
        }
        else
        {
            pullRecipe = null;
        }
        recipe = recipeMap.GetRecipe(handItem, false, groundItem);
        if (recipe != null)
        {
            pushAction = Action.Craft;
            pushRecipe = recipe;
        }
        else
        {
            pushRecipe = null;
        }

        UpdateIndicators();
    }

    void UpdateIndicators()
    {
        if (pullAction != Action.None)
        {
            leftIndicator.SetActive(true);
        }
        else
        {
            leftIndicator.SetActive(false);
        }
        if (pushAction != Action.None)
        {
            rightIndicator.SetActive(true);
        }
        else
        {
            rightIndicator.SetActive(false);
        }
    }

    void FindGroundWithRay()
    {
        Ray ray = new Ray(cam.position, cam.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 5f, tileMask))
        {
            if (hit.collider.GetComponent<TileObject>())
            {
                if (tileObject != hit.collider.GetComponent<TileObject>())
                {
                    //Debug.Log("looked at new tile");
                    tileObject = hit.collider.GetComponent<TileObject>();
                    depth = 0;
                    if (tileObject.itemSlot != null)
                    {
                        itemObject = tileObject.itemSlot;
                    }
                    else
                    {
                        itemObject = null;
                    }
                    UpdateMarker(tileObject.transform);
                    FindActions();
                }
            }
            else if (hit.collider.GetComponent<ItemObject>())
            {
                if (itemObject != hit.collider.GetComponent<ItemObject>())
                {
                    //Debug.Log("looked at new item");
                    itemObject = hit.collider.GetComponent<ItemObject>();
                    if (hit.collider.transform.parent.GetComponent<TileObject>()) //change to while loop
                    {
                        tileObject = hit.collider.transform.parent.GetComponent<TileObject>();
                        depth = 0;
                    }
                    else if (hit.collider.transform.parent.transform.parent.GetComponent<TileObject>())
                    {
                        tileObject = hit.collider.transform.parent.parent.GetComponent<TileObject>();
                        depth = 1;
                    }
                    else if (hit.collider.transform.parent.parent.transform.parent.GetComponent<TileObject>())
                    {
                        tileObject = hit.collider.transform.parent.parent.parent.GetComponent<TileObject>();
                        depth = 2;
                    }
                    UpdateMarker(tileObject.transform);
                    FindActions();
                }
            }
        }
        else if (tileObject != null)
        {
            //Debug.Log("ground set to null");
            tileObject = null;
            itemObject = null;
            UpdateMarker(null);
            FindActions();
        }
    }

    void UpdateMarker(Transform t)
    {
        marker.parent = t;
        marker.localPosition = Vector3.zero;
    }
    void UpdateMarkerRotation(int rotation)
    {
        marker.localEulerAngles = new Vector3(0, rotation, 0);
    }

    enum Action
    {
        None,
        Grab,
        Drop,
        Craft,
        Dig,
        Fill,
        Interact
    }
}
