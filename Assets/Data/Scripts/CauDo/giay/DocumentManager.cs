//using System.Collections.Generic;
//using UnityEngine;

//public class DocumentManager : MonoBehaviour
//{
//    public static DocumentManager instance;

//    [Header("UI Components")]
//    [Tooltip("Prefab của cái nút bấm nhỏ để hiện trong kho")]
//    public GameObject collectedButtonPrefab;
//    [Tooltip("Panel chứa các nút bấm đã thu thập (có Layout Group)")]
//    public Transform buttonContainer;

//    private List<PaperInteraction> collectedPapers = new List<PaperInteraction>();
//    private PaperInteraction currentlyViewingPaper = null;

//    void Awake()
//    {
//        if (instance == null) instance = this;
//        else Destroy(gameObject);
//    }

//    void Update()
//    {
//        if (currentlyViewingPaper != null && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.F)))
//        {
//            currentlyViewingPaper.CloseFromManager();
//            currentlyViewingPaper = null;
//        }
//    }

//    public void CollectPaper(PaperInteraction paper)
//    {
//        if (!collectedPapers.Contains(paper))
//        {
//            collectedPapers.Add(paper);
//            GameObject buttonGO = Instantiate(collectedButtonPrefab, buttonContainer);
//            buttonGO.GetComponent<CollectedPaperButton>().Setup(paper);
//        }
//    }

//    public void ShowPaperFromInventory(PaperInteraction paper)
//    {
//        if (currentlyViewingPaper != null && currentlyViewingPaper != paper)
//        {
//            currentlyViewingPaper.CloseFromManager();
//        }
//        currentlyViewingPaper = paper;
//        paper.ShowFromManager();
//    }
//}
using System.Collections.Generic;
using UnityEngine;

public class DocumentManager : MonoBehaviour
{
    public static DocumentManager instance;

    [Header("UI Components")]
    public GameObject collectedButtonPrefab;
    public Transform buttonContainer;

    private List<PaperInteraction> collectedPapers = new List<PaperInteraction>();
    private PaperInteraction currentlyViewingPaper = null;

    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        if (currentlyViewingPaper != null && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.F)))
        {
            // --- PHẦN MỚI: PHÁT ÂM THANH KHI ĐÓNG ---
            currentlyViewingPaper.PlayCloseSound();
            // ---------------------------------------
            currentlyViewingPaper.CloseFromManager();
            currentlyViewingPaper = null;
        }
    }

    public void CollectPaper(PaperInteraction paper)
    {
        if (!collectedPapers.Contains(paper))
        {
            collectedPapers.Add(paper);
            GameObject buttonGO = Instantiate(collectedButtonPrefab, buttonContainer);
            buttonGO.GetComponent<CollectedPaperButton>().Setup(paper);
        }
    }

    public void ShowPaperFromInventory(PaperInteraction paper)
    {
        if (currentlyViewingPaper != null && currentlyViewingPaper != paper)
        {
            currentlyViewingPaper.PlayCloseSound();
            currentlyViewingPaper.CloseFromManager();
        }

        currentlyViewingPaper = paper;
        // --- PHẦN MỚI: PHÁT ÂM THANH KHI MỞ ---
        paper.PlayOpenSound();
        // -------------------------------------
        paper.ShowFromManager();
    }
}

