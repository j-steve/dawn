using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapCamera : MonoBehaviour
{

    public float panSpeedMinZoom, panSpeedMaxZoom;
    public float fastPanSpeed;

    public float stickMinZoom, stickMaxZoom;

    public float swivelMinZoom, swivelMaxZoom;

    Transform swivel, stick;


    float zoom = 1f;


    // Use this for initialization
    void Awake()
    {
        swivel = transform.GetChild(0);
        stick = swivel.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        float zoomDelta = Input.GetAxis("Mouse ScrollWheel");
        if (zoomDelta != 0f) {
            AdjustZoom(zoomDelta);
        }
        float xDelta = Input.GetAxis("Horizontal");
        float zDelta = Input.GetAxis("Vertical");
        if (xDelta != 0f || zDelta != 0f) {
            bool isFastPan = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            AdjustPosition(xDelta, zDelta, isFastPan ? fastPanSpeed : 1);
        }
    }

    void AdjustZoom(float delta)
    {
        zoom = Mathf.Clamp01(zoom + delta);
        float distance = Mathf.Lerp(stickMinZoom, stickMaxZoom, zoom);
        stick.localPosition = new Vector3(0f, 0f, distance);

        float angle = Mathf.Lerp(swivelMinZoom, swivelMaxZoom, zoom);
        swivel.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }

    void AdjustPosition(float xDelta, float zDelta, float speedMultiplier)
    {
        Vector3 direction = new Vector3(xDelta, 0f, zDelta).normalized;
        float damping = Mathf.Max(Mathf.Abs(xDelta), Mathf.Abs(zDelta));
        float speed = Mathf.Lerp(panSpeedMinZoom, panSpeedMaxZoom, zoom);
        float distance = speed * speedMultiplier * damping * Time.deltaTime;

        Vector3 position = ClampPosition(transform.localPosition);
        position += direction * distance;
        transform.localPosition = position;
    }

    Vector3 ClampPosition(Vector3 position)
    {
        //var mapSize = HexBoard.ActiveBoard.mapSize * HexConstants.CELLS_PER_CHUNK_ROW;
        //float xMax = (mapSize.x - 0.5f) * HexConstants.HEX_RADIUS * 2f;
        //float zMax = (mapSize.y - 1f) * HexConstants.HEX_RADIUS * 1.5f;
        //HexCell lastCell = HexBoard.ActiveBoard.hexCells.Last().Value;
        //position.x = Mathf.Clamp(0, 50, lastCell.Center.x);
        //position.z = Mathf.Clamp(0, 50, lastCell.Center.z);
        return position;
    }
}
