using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[System.Serializable]
public class Interactive : MonoBehaviour
{
    [SerializeField] GameObject tip;
    [SerializeField] GameObject highLight;
    public bool isRepeat;
    public bool isInteracted;
    protected bool isInteracting;
    public bool isIntertactable;
    protected Collider2D coll;

    protected virtual void Awake()
    {
        coll = GetComponent<Collider2D>();
        highLight.GetComponent<Light2D>().lightCookieSprite = GetComponent<SpriteRenderer>().sprite;
    }

    protected virtual void Update()
    {
        if (isInteracted && !isRepeat) return;
        if (isIntertactable)
        {
            if (!Input.GetButtonDown("Interact") || isInteracting) return;
            isInteracted = true;
            isInteracting = true;
            Intertact();
            tip.SetActive(false);
            highLight.SetActive(false);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (isInteracted && !isRepeat) return;
        GameManager.Add_InteractableList(this);
    }

    protected virtual void OnTriggerExit2D(Collider2D collision)
    {
        tip.SetActive(false);
        highLight.SetActive(false);
        isIntertactable = false;
        isInteracting = false;
        GameManager.Remove_InteractableList(this);
    }

    //交互
    protected virtual void Intertact() {}
    //设为可交互状态
    public IEnumerator Set_Interactable()
    {
        if (isInteracted&&!isRepeat) yield break;
        tip.SetActive(true);
        highLight.SetActive(true);
        yield return null;
        isIntertactable = true;
    }
}
