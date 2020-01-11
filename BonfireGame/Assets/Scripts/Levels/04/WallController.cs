using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallController : MonoBehaviour
{
    public WallMover[] walls;
    public float cooldown = 5f;
    public StopButton stopButton;

    private bool _status;

    private void Start()
    {
        _status = true;
    }
    public void stopAllWalls()
    {
        if (_status)
        {
            for (int i = 0; i < walls.Length; i++)
            {
                walls[i].stopMoving();
            }

            _status = false;
            StartCoroutine(waitCooldown());
        }
    }

    public void startAllWalls()
    {
        if (!_status)
        {
            for (int i = 0; i < walls.Length; i++)
            {
                walls[i].startMoving();
            }
        }
    }

    public void resetAllWalls()
    {
        for (int i = 0; i < walls.Length; i++)
        {
            walls[i].resetWall();
        }
        _status = false;
    }

    private IEnumerator waitCooldown()
    {
        yield return new WaitForSeconds(cooldown);
        startAllWalls();
        _status = true;
        stopButton.swaper.resetSprite();
        stopButton.lightSprite.color = Color.red;
    }
}
