using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;
    public GameObject[] initialPartyMembers;

    public List<GameObject> partyMembers;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        InitializePartyMembers();
    }

    public void InitializePartyMembers()
    {
        foreach (GameObject member in initialPartyMembers)
            AddPartyMember(member);
    }

    public bool AddPartyMember(GameObject member)
    {
        GameObject memberObject = Instantiate(member);
        DontDestroyOnLoad(memberObject);
        memberObject.SetActive(false);
        partyMembers.Add(memberObject);
        return true;
    }
}