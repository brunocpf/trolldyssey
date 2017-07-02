using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;
    public GameObject[] initialPartyMembers;

    public List<GameObject> partyMembers;

    private void Awake()
    {
        Debug.Log("Awoken!");
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        ClearParty();
        InitializePartyMembers();
    }

    public void ClearParty()
    {
        foreach (GameObject member in partyMembers)
        {
            Debug.Log(member.name);
            partyMembers.Remove(member);
            Destroy(member);
        }
        partyMembers.Clear();
        partyMembers = new List<GameObject>();
    }

    public void InitializePartyMembers()
    {
        foreach (GameObject member in initialPartyMembers)
            AddPartyMember(member);
    }

    public bool AddPartyMember(GameObject member)
    {
        GameObject memberObject = Instantiate(member);
        //DontDestroyOnLoad(memberObject);
        memberObject.SetActive(false);
        partyMembers.Add(memberObject);
        return true;
    }
    
}