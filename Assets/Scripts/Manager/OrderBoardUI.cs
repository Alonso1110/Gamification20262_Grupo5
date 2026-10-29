
using System.Collections.Generic;
using UnityEngine;

public class OrderBoardUI : MonoBehaviour
{
    public static OrderBoardUI Instance { get; private set; }

    [SerializeField] private Transform ticketContainer;
    [SerializeField] private GameObject orderTicketPrefab;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void UpdateBoard()
    {
        if (ticketContainer == null || orderTicketPrefab == null) return;

        foreach (Transform child in ticketContainer)
            Destroy(child.gameObject);

        List<Order> orders = null;

        if (TutorialOrderManager.Instance != null)
            orders = TutorialOrderManager.Instance.activeOrders;
        else if (OrderManager.Instance != null)
            orders = OrderManager.Instance.activeOrders;

        if (orders == null) return;

        int orderNumber = 1;

        foreach (Order order in orders)
        {
            GameObject ticketObj = Instantiate(orderTicketPrefab, ticketContainer);

            OrderTicketUI ticketUI = ticketObj.GetComponent<OrderTicketUI>();

            if (ticketUI != null)
                ticketUI.Setup(order, orderNumber);

            orderNumber++;
        }
    }
}
