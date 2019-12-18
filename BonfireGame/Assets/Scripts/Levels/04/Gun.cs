using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public Transform shootPos;
    public GameObject shootPref;
    public float bulletSpeed = 2f;
    public Slider slider;
    public float cooldown;
    public SpriteRenderer trunkSprite;
    public float maxZ;
    public float minZ;

    public bool isUP { get; set; }
    public bool isDown { get; set; }

    private bool _fixPower;
    private float _bulletVelocity;
    private float _currRot;
    private float _rotStep = 0.6f;

    private void Start()
    {
        isUP = false;
        isDown = false;
        _fixPower = false;
        _bulletVelocity = 0f;
        Debug.Log(trunkSprite.transform.rotation.eulerAngles.z);
    }
    private void Update()
    {
        powerShoot();

        if (isUP)
        {
            upRotateTrunk();
        }

        if (isDown)
        {
            downRotateTrunk();
        }
    }

    public void shooting()
    {
        _fixPower = true;
        _bulletVelocity *= 20;
        GameObject bullet = Instantiate(shootPref, shootPos.position, Quaternion.identity);
        Rigidbody2D bulletRb = bullet.GetComponent<Rigidbody2D>();
        bulletRb.AddRelativeForce(shootPos.transform.TransformDirection
            (new Vector2((Mathf.Cos(shootPos.transform.rotation.z * Mathf.Deg2Rad) * _bulletVelocity),
            (Mathf.Sin(shootPos.transform.rotation.z * Mathf.Deg2Rad) * _bulletVelocity))),
            ForceMode2D.Impulse);

        StartCoroutine(shootCooldown());
    }

    public void powerShoot()
    {
        if (!_fixPower)
        {
            slider.value = Mathf.MoveTowards(slider.value, 100f, 0.01f);
            _bulletVelocity = slider.value;
            if (slider.value == 1f)
            {
                slider.value = 0f;
            }
        }
    }

    private IEnumerator shootCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        _fixPower = false;
    }

    public void upRotateTrunk()
    {
        maxZ = 40f;
        _currRot = trunkSprite.transform.rotation.eulerAngles.z;
        if (_currRot < maxZ)
        {
            trunkSprite.transform.Rotate(0f, 0f, _rotStep);
            Debug.Log(_currRot);
        }
    }

    public void downRotateTrunk()
    {
        minZ = 0f;
        _currRot = trunkSprite.transform.rotation.eulerAngles.z;
        if (_currRot > minZ + _rotStep)
        {
            trunkSprite.transform.Rotate(0f, 0f, -_rotStep);
            Debug.Log(_currRot);
        }
    }
}
