using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Rocket : MonoBehaviour
{
    public Transform[] roketSmokesPos;
    public Transform roketFirePos;
    public ParticleSystem smokeParticle;
    public ParticleSystem fireParticle;

    private ParticleSystem _roketEffect;

    private void Awake()
    {
        this.enabled = false;
    }
    void Start()
    {
        var smoke = smokeParticle.main;
        smoke.loop = false;
        _roketEffect = roketFireing();
    }

    private void OnEnable()
    {
        StartCoroutine(roketGonafly());
    }

    private IEnumerator roketGonafly()
    {
        for (int i = 0; i < roketSmokesPos.Length; i++)
        {
            ParticleSystem smoke = Instantiate(smokeParticle, roketSmokesPos[i].position, roketSmokesPos[i].rotation);
            smoke.Play();
            Destroy(smoke.gameObject, 1f);

            yield return new WaitForSeconds(1f);
        }

        roketFly();
        _roketEffect.Play();

        GameManager.Instance.addWinKey();
        GameManager.Instance.getWinReward();
    }
    private void roketFly()
    {
        transform.DOMoveY(12f, 4.5f);
        transform.DORotate(new Vector3(0, 0, Random.Range(-7f, 7f)), 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
    }
    private ParticleSystem roketFireing()
    {
        ParticleSystem roketFire = Instantiate(fireParticle, roketFirePos.position, fireParticle.transform.rotation);
        roketFire.transform.SetParent(roketFirePos);
        return roketFire;
    }
}
