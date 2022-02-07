using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

/// <summary>
/// Handles creation of a shuffled card deck and ordering of cards to different states of use.
/// </summary>

public class CardDeckHandler : MonoBehaviour
{
    [SerializeField]
    private int m_cardDealAmount = 3;

    private List<int> m_idDeck = new List<int>();
    private List<CardInstance> m_cardsDealt = new List<CardInstance>();
    private List<CardInstance> m_cardsInPlay = new List<CardInstance>();

    public GameObject m_cardUIObjectHolder;

    public GameObject[] m_cardUIObjects;

    private Queue<int> m_shuffledIDDeck = new Queue<int>();

    private CardAsset[] m_cardAssets;

    private World m_world;

    private GameManager m_gameManager;

    [SerializeField]
    private PlayedCardHolder m_playedCardHolder;

    private void Awake()
    {
        m_world = FindObjectOfType<World>();

        // Reset leftover data 
        m_idDeck.Clear();

        // Load the deck of card IDs
        LoadDeck();
    }

    private void Start()
    {
        //get game manager
        m_gameManager = GameManager.Instance;

        m_shuffledIDDeck.Clear();
        m_cardsDealt.Clear();
        m_cardsInPlay.Clear();

        // Shuffle deck of card IDs once at the beginning of the game
        ShuffleDeck(m_idDeck);

        // Transfer Cards into a Queue for ease of access
        foreach (int card in m_idDeck)
        {
            m_shuffledIDDeck.Enqueue(card);
        }
    }

    private void LoadDeck()
    {
        m_cardAssets = Resources.LoadAll<CardAsset>("Cards");

        foreach (CardAsset currentCardAsset in m_cardAssets)
        {
            int cardID = currentCardAsset.m_id;
            int cardQuantity = currentCardAsset.m_quantity;

            for (int q = 0; q < cardQuantity; q++)
            {
                m_idDeck.Add(cardID);
            }
        }
        // Check the size of the deck
        //Debug.Log($"Deck size = {m_iDDeck.Count}");
    }

    private List<int> ShuffleDeck(List<int> deckIDs)
    {
        for (int i = 0; i < deckIDs.Count; i++)
        {
            int randomIndex = Random.Range(0, deckIDs.Count);
            int temp = deckIDs[i];
            deckIDs[i] = deckIDs[randomIndex];
            deckIDs[randomIndex] = temp;
        }
        return deckIDs;
    }

    private CardInstance CreateCardInstance(int id)
    {
        CardInstance dealtCard = new CardInstance
        {
            cardAssetID = id,
            durationRemaining = m_cardAssets[id].m_effectDuration
        };

        return dealtCard;
    }

    public void DealCards()
    {
        for (int i = 0; i < m_cardDealAmount; i++)
        {
            // Dequeue cards from the deck and add to list of cards dealt
            if (m_shuffledIDDeck.Count != 0)
            {
                CardInstance cardDealt = CreateCardInstance(m_shuffledIDDeck.Dequeue());
                m_cardsDealt.Add(cardDealt);

                // Activate UI card 
                m_cardUIObjects[i].SetActive(true);

                // Populate card UI elements with data from Card
                m_cardUIObjects[i].transform.Find("Card Name").GetComponent<TMP_Text>().text = m_cardAssets[cardDealt.cardAssetID].m_name;
                m_cardUIObjects[i].transform.Find("Card Description").GetComponent<TMP_Text>().text = m_cardAssets[cardDealt.cardAssetID].m_description;
                // etc. all other info passed here
                // Ex:
                //m_cardUIObjects[i].transform.Find("Card Image").GetComponent<Image>().sprite = m_cardAssets[cardDealt.cardAssetID].m_sprite;
                //m_cardUIObjects[i].transform.Find("Cost").GetComponent<Text>().text = m_cardAssets[cardDealt.cardAssetID].m_initialCost.ToString();
            }
        }
    }

    public void PlayCard(GameObject chosenCard)
    {
        for (int i = 0; i < m_cardsDealt.Count; i++)
        {
            if (chosenCard.name == m_cardUIObjectHolder.transform.GetChild(i).name)
            {
                m_cardsInPlay.Add(m_cardsDealt[i]);

                m_playedCardHolder.AddNewCard(m_cardUIObjects[i].transform.Find("Card Name").GetComponent<TMP_Text>().text, m_cardUIObjects[i].transform.Find("Card Description").GetComponent<TMP_Text>().text);

            }
        }
        DiscardDealtCards();
        m_world.ChangeSeason(SeasonState.Summer);
      
    }

    private void DiscardDealtCards()
    {
        for (int i = 0; i < m_cardDealAmount; i++)
        {
            m_cardUIObjects[i].SetActive(false);
        }

        m_cardsDealt.Clear();
    }

    // TODO: 
    public void DiscardPlayedCards()
    {

    }
}
