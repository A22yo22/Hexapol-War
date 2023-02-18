using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldData : MonoBehaviour
{
    [Header("Properties")]
    public CaptureState fieldMode = CaptureState.Clear;

    [Header("Colors")]
    [SerializeField] Material clearColor;
    [SerializeField] Material blueColor;
    [SerializeField] Material redClor;
    [SerializeField] Material yellowClor;

    public enum CaptureState
    {
        Clear,
        Player1,
        Player2,
        Select
    }

    public void SwitchCaptureState(CaptureState state)
    {
        switch (state)
        {
            case CaptureState.Clear:
                GetComponent<MeshRenderer>().material = clearColor;
                fieldMode = CaptureState.Clear;
                break;

            case CaptureState.Player1:
                GetComponent<MeshRenderer>().material = blueColor;
                fieldMode = CaptureState.Player1;
                break;

            case CaptureState.Player2:
                GetComponent<MeshRenderer>().material = redClor;
                fieldMode = CaptureState.Player2;
                break;

            case CaptureState.Select:
                GetComponent<MeshRenderer>().material = yellowClor;
                fieldMode = CaptureState.Select;
                break;
        }
    }
}
