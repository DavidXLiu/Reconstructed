using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StringInput : MonoBehaviour
{
    #region Inspector Fields
    public List<Text> letterTexts;
    public Text nameText;
    #endregion

    private char[] nameChars = new char[15]; // Cap the name to be 15 characters long 
    private bool upperCase = false;

    // Start is called before the first frame update
    void Start()
    {
        // Initialize with null characters
        for (int i = 0; i < nameChars.Length; i++)
            nameChars[i] = '`';

        DisplayNameText();
    }

    // Update is called once per frame
    void Update()
    {
        // Check for shift case
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if (!upperCase)
                upperCase = true;
        }

        // Only switch back on releasing shift
        if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            upperCase = false;
        }

        // Type in all characters through keyboard
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (upperCase)
                EnterChar('A');
            else
                EnterChar('a');
        }
        else if (Input.GetKeyDown(KeyCode.B))
        {
            if (upperCase)
                EnterChar('B');
            else
                EnterChar('b');
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            if (upperCase)
                EnterChar('C');
            else
                EnterChar('c');
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            if (upperCase)
                EnterChar('D');
            else
                EnterChar('d');
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            if (upperCase)
                EnterChar('E');
            else
                EnterChar('e');
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            if (upperCase)
                EnterChar('F');
            else
                EnterChar('f');
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            if (upperCase)
                EnterChar('G');
            else
                EnterChar('g');
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            if (upperCase)
                EnterChar('H');
            else
                EnterChar('h');
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            if (upperCase)
                EnterChar('I');
            else
                EnterChar('i');
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            if (upperCase)
                EnterChar('J');
            else
                EnterChar('j');
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            if (upperCase)
                EnterChar('K');
            else
                EnterChar('k');
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            if (upperCase)
                EnterChar('L');
            else
                EnterChar('l');
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            if (upperCase)
                EnterChar('M');
            else
                EnterChar('m');
        }
        else if (Input.GetKeyDown(KeyCode.N))
        {
            if (upperCase)
                EnterChar('N');
            else
                EnterChar('n');
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            if (upperCase)
                EnterChar('O');
            else
                EnterChar('o');
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            if (upperCase)
                EnterChar('P');
            else
                EnterChar('p');
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            if (upperCase)
                EnterChar('Q');
            else
                EnterChar('q');
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            if (upperCase)
                EnterChar('R');
            else
                EnterChar('r');
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            if (upperCase)
                EnterChar('S');
            else
                EnterChar('s');
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            if (upperCase)
                EnterChar('T');
            else
                EnterChar('t');
        }
        else if (Input.GetKeyDown(KeyCode.U))
        {
            if (upperCase)
                EnterChar('U');
            else
                EnterChar('u');
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            if (upperCase)
                EnterChar('V');
            else
                EnterChar('v');
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            if (upperCase)
                EnterChar('W');
            else
                EnterChar('w');
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            if (upperCase)
                EnterChar('X');
            else
                EnterChar('x');
        }
        else if (Input.GetKeyDown(KeyCode.Y))
        {
            if (upperCase)
                EnterChar('Y');
            else
                EnterChar('y');
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            if (upperCase)
                EnterChar('Z');
            else
                EnterChar('z');
        }
        else if (Input.GetKeyDown(KeyCode.Question))
        {
            EnterChar('?');
        }
        else if (Input.GetKeyDown(KeyCode.Exclaim))
        {
            EnterChar('!');
        }
        else if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            EnterChar(';');
        }
        else if (Input.GetKeyDown(KeyCode.Colon))
        {
            EnterChar(':');
        }
        else if (Input.GetKeyDown(KeyCode.Quote))
        {
            EnterChar('\'');
        }
        else if (Input.GetKeyDown(KeyCode.DoubleQuote))
        {
            EnterChar('"');
        }
        else if (Input.GetKeyDown(KeyCode.Comma))
        {
            EnterChar(',');
        }
        else if (Input.GetKeyDown(KeyCode.Period))
        {
            EnterChar('.');
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            EnterChar(' ');
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            DeleteChar();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            // To Do: Confirm
        }
    }

    #region Event Handlers
    public void EnterChar(Text text)
    {
        // Go through the array finding where to place the character
        for (int i = 0; i < nameChars.Length; i++)
        {
            // Find the first available character that isn't null
            if (nameChars[i] == '`')
            {
                // Special case for spaces
                if (text.text[0] == '_')
                    nameChars[i] = ' ';
                else
                    nameChars[i] = text.text[0];
                break;
            }
        }

        // Update the name display
        DisplayNameText();
    }

    public void EnterChar(char c)
    {
        // Go through the array finding where to place the character
        for (int i = 0; i < nameChars.Length; i++)
        {
            // Find the first available character that isn't null
            if (nameChars[i] == '`')
            {
                // Special case for spaces
                if (c == '_')
                    nameChars[i] = ' ';
                else
                    nameChars[i] = c;
                break;
            }
        }

        // Update the name display
        DisplayNameText();
    }

    public void DeleteChar()
    {
        // Remove the last element in the array that isn't null
        for (int i = nameChars.Length - 1; i >= 0; i--)
        {
            // Identify the most recent character
            if (nameChars[i] != '`')
            {
                nameChars[i] = '`';
                break;
            }
        }

        // Update the name display
        DisplayNameText();
    }

    public void ShiftChange()
    {
        // Change case
        upperCase = !upperCase;

        // Change all letters
        foreach (Text text in letterTexts)
        {
            // Change to upper case
            if (upperCase)
                text.text = text.text.ToUpper();
            // Change to lower case
            else
                text.text = text.text.ToLower();
        }
    }
    #endregion

    #region Helper Methods
    private void DisplayNameText()
    {
        /// The '`' character is treated as null

        // Display first character
        if (nameChars[0] == '`')
            nameText.text = "_";
        else
            nameText.text = $"{nameChars[0]}";

        // Display the current input for the name
        for (int i = 1; i < nameChars.Length; i++)
        {
            if (nameChars[i] == '`')
                nameText.text += " _";
            else
                nameText.text += $" {nameChars[i]}";
        }
    }
    #endregion
}
