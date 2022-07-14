using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour
{
    [SerializeField] private Transform cameraArm;
    [SerializeField] private Transform rocketArm;
    [SerializeField] private Transform bulletSpawnLocation;
    
    private Rigidbody _rigidbody;
    private LineRenderer _lineRenderer;
    
    private bool _isGrounded;
    private float _bulletTime;

    private RaycastHit hitData;

    public float speed;
    public float jumpPower;
    public float mouseSensitivity;
    public float shotInterval;

    public GameObject _bullet;
    public Camera _camera;

    private void Start()
    {
        Cursor.visible = false;
        _rigidbody = GetComponentInChildren<Rigidbody>();
        _lineRenderer = GetComponentInChildren<LineRenderer>();
    }

    private void Update()
    {
        _bulletTime -= Time.deltaTime;
        LookAround();
        DrawBulletTraj();
    }

    public void InputProcessing(float hValue, float vValue, bool space, bool leftMouse)
    {
        // Move (WASD)
        var direction = new Vector3(cameraArm.forward.x, 0f, cameraArm.forward.z).normalized;
        var orthoDir = new Vector3(direction.z, 0f, -direction.x).normalized;
        
        Debug.DrawRay(cameraArm.position,
            direction, Color.blue);

        var movingDirection
            = (direction * vValue + orthoDir * hValue).normalized;
        
        transform.Translate(movingDirection * (speed * Time.deltaTime));
        
        // Jump (Space)
        if (space) Debug.Log("SPACED");
        if (_isGrounded)
        {
            if (space)
            {
                _isGrounded = false;
                _rigidbody.AddForce(new Vector3(0f, jumpPower, 0f), ForceMode.Impulse);
            }
        }
        
        // Shot (Left Mouse)
        if (leftMouse)
        {
            Shot();
        }
    }

    private void LookAround() // Camera look at mouse
    {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSensitivity;
        Vector3 camAngle = cameraArm.rotation.eulerAngles;

        var x = camAngle.x - mouseDelta.y;
        if (x < 180f)
        {
            x = Mathf.Clamp(x, -1f, 70f);
        }
        else
        {
            x = Mathf.Clamp(x, 335f, 361f);
        }

        cameraArm.rotation = Quaternion.Euler(x, camAngle.y + mouseDelta.x, camAngle.z);

        RaycastHit hit1, hit2;

        var ray1 = _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        var ray2 = new Ray(bulletSpawnLocation.position, cameraArm.forward);

        Vector3 dst = cameraArm.forward;
        if (Physics.Raycast(ray2, out hit2, 100f))
        {
            dst = hit2.point - transform.position;
        } 
        if (Physics.Raycast(ray1, out hit1, 100f))
        {
            dst = hit1.point - transform.position;
        } 
        
        rocketArm.rotation = Quaternion.Lerp(rocketArm.rotation, Quaternion.LookRotation(dst), Time.deltaTime * 10);
    }

    private void Shot()
    {
        if (_bulletTime < 0)
        {
            _bulletTime = shotInterval;
            GameObject bullet = GameObject.Instantiate(_bullet, bulletSpawnLocation.position, rocketArm.rotation);
            BulletBehavior behavior = bullet.GetComponent<BulletBehavior>();
            behavior.velocity = 20f;
        }
    }

    private void DrawBulletTraj()
    {
        _lineRenderer.SetColors(Color.red, Color.red);
        _lineRenderer.SetWidth(0.01f, 0.01f);
        _lineRenderer.SetPosition(0, bulletSpawnLocation.position);
        
        if (Physics.Raycast(bulletSpawnLocation.position, rocketArm.forward, out hitData, 100f))
        {
            _lineRenderer.SetPosition(1, hitData.point);
        }
        else
        {
            _lineRenderer.SetPosition(1, bulletSpawnLocation.position + 100f * rocketArm.forward);
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.contacts[0].normal.y > 0.9) 
        {
            if (collision.gameObject.tag == "Ground")
            {
                _isGrounded = true;
            }
        }
    }
}
