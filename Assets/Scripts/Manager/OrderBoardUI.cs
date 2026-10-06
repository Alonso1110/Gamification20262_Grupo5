using UnityEngine;

public class OrderBoardUI : MonoBehaviour
{
    public static OrderBoardUI Instance { get; private set; }

    [SerializeField] private Transform ticketContainer;
    [SerializeField] private GameObject orderTicketPrefab;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void UpdateBoard()
    {
        if (ticketContainer == null || orderTicketPrefab == null) return;

        foreach (Transform child in ticketContainer)
        {
            child.gameObject.SetActive(false);
            Destroy(child.gameObject);
        }

        if (OrderManager.Instance != null)
        {
            int orderNumber = 1;
            foreach (Order order in OrderManager.Instance.activeOrders)
            {
                GameObject ticketObj = Instantiate(orderTicketPrefab, ticketContainer);
                OrderTicketUI ticketUI = ticketObj.GetComponent<OrderTicketUI>();
                if (ticketUI != null)
                    ticketUI.Setup(order, orderNumber);
                orderNumber++;
            }
        }
    }
}