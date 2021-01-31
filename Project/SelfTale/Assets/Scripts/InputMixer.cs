using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class InputMixer : MonoBehaviour
{
    static bool right;
    static bool left;


    static bool jump = false;
    static bool jumpLast;

    static bool jumpUp;
    static bool jumpDown;

    static bool crouch;

    static bool skill1;
    static bool skill1Last;

    static bool skill1Down;


    static bool skill2;
    static bool skill2Last;

    static bool skill2Down;


    static bool skill3;
    static bool skill3Last;

    static bool skill3Down;


    static bool dash;
    static bool dashLast;

    static bool dashDown;

    static bool en;
    static bool enLast;

    static bool enDown;

    static bool back;
    static bool backDown;
    static bool backLast;

    private void Update()
    {
        if (jumpLast == false && jump == true)
        {
            jumpDown = true;
        }
        else
        {
            jumpDown = false;
        }
        if (jumpLast == true && jump == false)
        {
            jumpUp = true;
        }
        else
        {
            jumpUp = false;
        }

        if (skill1Last == false && skill1 == true)
        {
            skill1Down = true;
        }
        else
        {
            skill1Down = false;
        }

        if (skill2Last == false && skill2 == true)
        {
            skill2Down = true;
        }
        else
        {
            skill2Down = false;
        }

        if (skill3Last == false && skill3 == true)
        {
            skill3Down = true;
        }
        else
        {
            skill3Down = false;
        }

        if (dashLast == false && dash == true)
        {
            dashDown = true;
        }
        else
        {
            dashDown = false;
        }


        if (enLast == false && en == true)
        {
            enDown = true;
        }
        else
        {
            enDown = false;
        }


        if (backLast == false && back == true)
        {
            backDown = true;
        }
        else
        {
            backDown = false;
        }

        jumpLast = jump;
        backLast = back;
        skill1Last = skill1;
        skill2Last = skill2;
        skill3Last = skill3;
        dashLast = dash;
        enLast = en;



    }

    public bool Right
    {
        get { return right; }
        set { right = value; }
    }
    public bool Left
    {
        get { return left; }
        set { left = value; }
    }
    public static float HorizontalS
    {
        get { if (left == right) return 0; else return (right) ? 1 : -1; }
    }

    public bool Jump
    {
        get { return jump; }
        set {jump = value; }
    }

    public static bool JumpS
    {
        get { return jump; }
        set { jump = value; }
    }


    public bool JumpDown
    {
        get { return jumpDown; }
    }
    public bool JumpUp
    {
        get { return jumpUp; }
    }

    public static bool JumpDownS
    {
        get { return jumpDown; }
    }
    public static bool JumpUpS
    {
        get { return jumpUp; }
    }


    public bool Crouch
    {
        get { return crouch; }
        set { crouch = value; }
    }
    public static bool CrouchS
    {
        get { return crouch; }
        set { crouch = value; }
    }


    public bool En
    {
        get { return en; }
        set { en = value; }
    }
    public bool EnDown
    {
        get { return enDown; }
    }

    public static bool EnS
    {
        get { return en; }
        set { en = value; }
    }
    public static bool EnDownS
    {
        get { return enDown; }
    }

    public bool Back
    {
        get { return back; }
        set { back = value; }
    }
    public bool BackDown
    {
        get { return backDown; }
    }

    public static bool BackS
    {
        get { return back; }
        set { back = value; }
    }
    public static bool BackDownS
    {
        get { return backDown; }
    }

    public bool Skill1
    {
        get { return skill1; }
        set { skill1 = value; }
    }
    public bool Skill1Down
    {
        get { return skill1Down; }
    }

    public static bool Skill1S
    {
        get { return skill1; }
        set { skill1 = value; }
    }
    public static bool Skill1DownS
    {
        get { return skill1Down; }
    }

    public bool Skill2
    {
        get { return skill2; }
        set { skill2 = value; }
    }
    public bool Skill2Down
    {
        get { return skill2Down; }
    }

    public static bool Skill2S
    {
        get { return skill2; }
        set { skill2 = value; }
    }
    public static bool Skill2DownS
    {
        get { return skill2Down; }
    }

    public bool Skill3
    {
        get { return skill3; }
        set { skill3 = value; }
    }
    public bool Skill3Down
    {
        get { return skill3Down; }
    }

    public static bool Skill3S
    {
        get { return skill3; }
        set { skill3 = value; }
    }
    public static bool Skill3DownS
    {
        get { return skill3Down; }
    }

    public bool Dash
    {
        get { return dash; }
        set { dash = value; }
    }
    public bool DashDown
    {
        get { return dashDown; }
    }

    public static bool DashS
    {
        get { return dash; }
        set { dash = value; }
    }
    public static bool DashDownS
    {
        get { return dashDown; }
    }
}
