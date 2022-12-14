
/*
This RPG data streaming assignment was created by Fernando Restituto.
Pixel RPG characters created by Sean Browning.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Debug = UnityEngine.Debug;


#region Assignment Instructions

/*  Hello!  Welcome to your first lab :)

Wax on, wax off.

    The development of saving and loading systems shares much in common with that of networked gameplay development.  
    Both involve developing around data which is packaged and passed into (or gotten from) a stream.  
    Thus, prior to attacking the problems of development for networked games, you will strengthen your abilities to develop solutions using the easier to work with HD saving/loading frameworks.

    Try to understand not just the framework tools, but also, 
    seek to familiarize yourself with how we are able to break data down, pass it into a stream and then rebuild it from another stream.


Lab Part 1

    Begin by exploring the UI elements that you are presented with upon hitting play.
    You can roll a new party, view party stats and hit a save and load button, both of which do nothing.
    You are challenged to create the functions that will save and load the party data which is being displayed on screen for you.

    Below, a SavePartyButtonPressed and a LoadPartyButtonPressed function are provided for you.
    Both are being called by the internal systems when the respective button is hit.
    You must code the save/load functionality.
    Access to Party Character data is provided via demo usage in the save and load functions.

    The PartyCharacter class members are defined as follows.  */

public partial class PartyCharacter
{
    public int classID;

    public int health;
    public int mana;

    public int strength;
    public int agility;
    public int wisdom;

    public LinkedList<int> equipment;

}


/*
    Access to the on screen party data can be achieved via …..

    Once you have loaded party data from the HD, you can have it loaded on screen via …...

    These are the stream reader/writer that I want you to use.
    https://docs.microsoft.com/en-us/dotnet/api/system.io.streamwriter
    https://docs.microsoft.com/en-us/dotnet/api/system.io.streamreader

    Alright, that’s all you need to get started on the first part of this assignment, here are your functions, good luck and journey well!
*/


#endregion


#region Assignment Part 1

static public class AssignmentPart1
{
    private const string mStatsSignifier = "0";
    private const string mEquipSignifier = "1";

    static public void SavePartyButtonPressed()
    {
        using (System.IO.StreamWriter mStreamWriter = new StreamWriter("SaveFile.txt"))
        {
            foreach (PartyCharacter pc in GameContent.partyCharacters)
            {
                mStreamWriter.WriteLine(mStatsSignifier + "," + pc.classID + "," + pc.health + "," + pc.mana + "," +
                                        pc.strength + "," + pc.agility + "," + pc.wisdom);

                foreach (int iCurrentEquip in pc.equipment)
                {
                    mStreamWriter.WriteLine(mEquipSignifier + "," + iCurrentEquip);
                }
            }
        }
        Debug.Log("Saved!");
    }

    static public void LoadPartyButtonPressed()
    {
        GameContent.partyCharacters.Clear();
        using (System.IO.StreamReader mStreamReader = new StreamReader("Savefile0.txt"))
        {
            string mCurrentLine = "";

            string[] sStats;
            int[] iCharacterStats = new int[7];

            string[] sEquipment;
            int[] iEquipmentID = new int[3];

            PartyCharacter pc = null;

            while ((mCurrentLine = mStreamReader.ReadLine()) != null)
            {
                if (mCurrentLine.StartsWith(mStatsSignifier))
                {

                    sStats = mCurrentLine.Split(",");
                    for (int i = 0; i < sStats.Length; i++)
                    {
                        iCharacterStats[i] = Int32.Parse(sStats[i]);
                    }

                    pc = new PartyCharacter(iCharacterStats[1], iCharacterStats[2], iCharacterStats[3],
                        iCharacterStats[4], iCharacterStats[5], iCharacterStats[6]);
                    GameContent.partyCharacters.AddLast(pc);
                }

                if (mCurrentLine.StartsWith('1'))
                {
                    Debug.Log("Line 2");
                    int[] iCharacterEquips = new int[2];

                    if (mCurrentLine.StartsWith(mEquipSignifier))
                    {
                        sEquipment = mCurrentLine.Split(",");
                        for (int i = 0; i < sEquipment.Length; i++)
                        {
                            iEquipmentID[i] = Int32.Parse(sEquipment[i]);
                        }

                        pc.equipment.AddLast(iEquipmentID[1]);
                    }
                }
            }

            GameContent.RefreshUI();
            Debug.Log("Loaded!");
        }
    }

}

#endregion


    #region Assignment Part 2

//  Before Proceeding!
//  To inform the internal systems that you are proceeding onto the second part of this assignment,
//  change the below value of AssignmentConfiguration.PartOfAssignmentInDevelopment from 1 to 2.
//  This will enable the needed UI/function calls for your to proceed with your assignment.
    static public class AssignmentConfiguration
    {
        public const int PartOfAssignmentThatIsInDevelopment = 2;
    }

/*

In this part of the assignment you are challenged to expand on the functionality that you have already created.  
    You are being challenged to save, load and manage multiple parties.
    You are being challenged to identify each party via a string name (a member of the Party class).

To aid you in this challenge, the UI has been altered.  

    The load button has been replaced with a drop down list.  
    When this load party drop down list is changed, LoadPartyDropDownChanged(string selectedName) will be called.  
    When this drop down is created, it will be populated with the return value of GetListOfPartyNames().

    GameStart() is called when the program starts.

    For quality of life, a new SavePartyButtonPressed() has been provided to you below.

    An new/delete button has been added, you will also find below NewPartyButtonPressed() and DeletePartyButtonPressed()

Again, you are being challenged to develop the ability to save and load multiple parties.
    This challenge is different from the previous.
    In the above challenge, what you had to develop was much more directly named.
    With this challenge however, there is a much more predicate process required.
    Let me ask you,
        What do you need to program to produce the saving, loading and management of multiple parties?
        What are the variables that you will need to declare?
        What are the things that you will need to do?  
    So much of development is just breaking problems down into smaller parts.
    Take the time to name each part of what you will create and then, do it.

Good luck, journey well.

*/

    static public class AssignmentPart2
    {
        private const string mStatsSignifier = "0";
        private const string mEquipSignifier = "1";
        private static List <string> mUserPartyListNames = new();
        private static string mCurrentSave = "";

        static public void GameStart()
        {
            GetFileNames();
            GameContent.RefreshUI();
        }

        static public void GetFileNames()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(), "Saves");
            string[] saveFileNames = Directory.GetFiles(path, "*.txt");

            for (int i = 0; i < saveFileNames.Length; i++)
            {
                saveFileNames[i] = Path.GetFileNameWithoutExtension(saveFileNames[i]);
            }

            foreach (string fileName in saveFileNames)
            {
                GetListOfPartyNames().Add(fileName);
            }
        }

        static public List<string> GetListOfPartyNames()
        {
            return mUserPartyListNames;
        }

        static public void LoadPartyDropDownChanged(string selectedName)
        {
            GameContent.partyCharacters.Clear();
            mCurrentSave = selectedName;
            using (System.IO.StreamReader mStreamReader = new StreamReader(@$"Saves\{selectedName}.txt"))
            {
                string mCurrentLine = "";

                string[] sStats;
                int[] iCharacterStats = new int[7];

                string[] sEquipment;
                int[] iEquipmentID = new int[3];

                PartyCharacter pc = null;
                while ((mCurrentLine = mStreamReader.ReadLine()) != null)
                {

                    Debug.Log("Current Line: " + mCurrentLine);
                    if (mCurrentLine.StartsWith(mStatsSignifier))
                    {

                        sStats = mCurrentLine.Split(",");
                        for (int i = 0; i < sStats.Length; i++)
                        {
                            iCharacterStats[i] = Int32.Parse(sStats[i]);
                        }

                        pc = new PartyCharacter(iCharacterStats[1], iCharacterStats[2], iCharacterStats[3],
                            iCharacterStats[4], iCharacterStats[5], iCharacterStats[6]);
                        GameContent.partyCharacters.AddLast(pc);
                    }

                    if (mCurrentLine.StartsWith(mEquipSignifier))
                    {
                        sEquipment = mCurrentLine.Split(",");
                        for (int i = 0; i < sEquipment.Length; i++)
                        {
                            iEquipmentID[i] = Int32.Parse(sEquipment[i]);
                        }

                        pc.equipment.AddLast(iEquipmentID[1]);
                    }

                    GameContent.RefreshUI();
                    Debug.Log("Loaded!");
                }
            }
        }

        static public void SavePartyButtonPressed()
        {
            string sNewSaveName = GameContent.GetPartyNameFromInput();
            using (System.IO.StreamWriter
                   mStreamWriter = new StreamWriter(@$"Saves\{sNewSaveName}.txt"))
            { 
                mUserPartyListNames.Add(sNewSaveName);
                foreach (PartyCharacter pc in GameContent.partyCharacters)
                {
                    mStreamWriter.WriteLine(mStatsSignifier + "," + pc.classID + "," + pc.health + "," + pc.mana + "," + pc.strength + "," + pc.agility + "," + pc.wisdom);
                    foreach (int iCurrentEquip in pc.equipment)
                    {
                        mStreamWriter.WriteLine(mEquipSignifier + "," + iCurrentEquip);
                    }
                }
            }
            Debug.Log($"{sNewSaveName} File Saved!");
            GameContent.RefreshUI();
        }

        static public void DeletePartyButtonPressed()
        {
            if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), @$"Saves\{mCurrentSave}.txt")))
            {
                File.Delete(@$"Saves\{mCurrentSave}.txt");
                mUserPartyListNames.Remove(mCurrentSave);
                GameContent.partyCharacters.Clear();
                GameContent.RerollParty();
                GameContent.RefreshUI();
                Debug.Log($"{mCurrentSave} has be deleted!");
            }
            else
            {
                Debug.Log("File not found.");
            }
        }
    }


#endregion


