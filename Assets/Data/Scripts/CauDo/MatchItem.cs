using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // Nhớ thêm dòng này để dùng Image
using UnityEngine.EventSystems;

public class MatchItem : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerEnterHandler, IPointerUpHandler, IPointerExitHandler
{
    public static MatchItem hoverItem;

    public GameObject linePrefab;
    public string itemName;

    private GameObject line;
    private RectTransform lineRectTransform; // Dùng RectTransform để scale chính xác hơn
    private Canvas parentCanvas; // Cache cái canvas lại để dùng

    void Start()
    {
        // Lấy canvas cha để tính toán tọa độ cho đúng
        parentCanvas = GetComponentInParent<Canvas>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (this.enabled == false) return; // Nếu đã nối rồi thì không làm gì cả

        // Tạo line và lấy RectTransform của nó
        line = Instantiate(linePrefab, transform.parent);
        lineRectTransform = line.GetComponent<RectTransform>();

        // Gọi hàm UpdateLine với eventData
        UpdateLine(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (line == null) return;

        // Cập nhật liên tục khi kéo
        UpdateLine(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (line == null) return;

        // Check xem có thả vào đúng đối tượng không
        if (hoverItem != null && hoverItem.enabled && !this.Equals(hoverItem) && itemName.Equals(hoverItem.itemName))
        {
            // Nối đường line tới vị trí của đối tượng đích
            UpdateLineToTarget(hoverItem.transform.position);

            // Tạm thời vô hiệu hóa 2 đối tượng đã nối thành công
            // Dùng Destroy(this) sẽ xóa mất script, có thể gây lỗi nếu muốn reset level
            //GetComponent<Image>().color = Color.gray; // Đổi màu để báo hiệu đã nối
            //hoverItem.GetComponent<Image>().color = Color.gray; // Đổi màu cả đối tượng đích
            this.enabled = false;
            hoverItem.enabled = false;
        }
        else
        {
            // Nếu thả ra ngoài hoặc sai đối tượng thì xóa line
            Destroy(line);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverItem = this;
    }

    // Hàm này sẽ được gọi khi không có chuột đi vào nữa, nên thêm vào cho chắc
    public void OnPointerExit(PointerEventData eventData)
    {
        hoverItem = null;
    }

    // --- CÁC HÀM UPDATE LINE ĐÃ SỬA ---

    // Hàm này nhận vào eventData để chuyển đổi tọa độ chuột
    void UpdateLine(PointerEventData eventData)
    {
        // Chuyển tọa độ chuột (screen space) sang tọa độ local trong canvas (world space)
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint
        );

        // Cập nhật vị trí của line
        UpdateLineToTarget(parentCanvas.transform.TransformPoint(localPoint));
    }

    // Hàm này sẽ định vị và xoay cái line tới vị trí đích
    void UpdateLineToTarget(Vector3 targetPosition)
    {
        // Vị trí bắt đầu là cục màu của mình
        Vector3 startPosition = transform.position;
        Vector3 direction = targetPosition - startPosition;

        // Đặt vị trí line ngay tại điểm bắt đầu (quan trọng là Pivot phải set Left)
        lineRectTransform.position = startPosition;
        // Xoay cái line hướng về phía target
        lineRectTransform.right = direction;

        // Scale chiều dài của line cho bằng khoảng cách
        // Phải chia cho scale của canvas để không bị ảnh hưởng bởi co giãn màn hình
        float distance = Vector3.Distance(startPosition, targetPosition);
        lineRectTransform.sizeDelta = new Vector2(distance / parentCanvas.transform.localScale.x, lineRectTransform.sizeDelta.y);
    }
}