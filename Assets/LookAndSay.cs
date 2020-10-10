using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using KModkit;

public class LookAndSay : MonoBehaviour
{
    public KMAudio bombaudio;
    public KMBombInfo bomb;
    public KMBombModule module;
    public KMSelectable[] buttons;
    public TextMesh ScreenText;



    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;
    private bool moduleReady = false;

    

    //Nasty way to define a maze, but it's important for the pathing algorithm, basically each cell is defined by all exits from the cell
    public List<char>[,] mazeA = new List<char>[5, 5] { {new List<char>() { 'R' },new List<char>() { 'L', 'R' },new List<char>() { 'L', 'R', 'D' },new List<char>() { 'L' },new List<char>() { 'D' } },
        { new List<char>() { 'D' },new List<char>() { 'D','R' }, new List<char>() {'U','R','D','L' },new List<char>() {'L','R' },new List<char>() {'L','U','D' } },
        {new List<char>() { 'U','R' },new List<char>() {'U','L' },new List<char>() {'U','R','D' },new List<char>() {'L','D' },new List<char>() {'U','D' } },
        {new List<char>() {'R' },new List<char>() {'L','D','R' },new List<char>() {'L','U' },new List<char>() {'U','D' },new List<char>() {'U','D' } },
        {new List<char>() {'R' },new List<char>() {'U','L'},new List<char>() {'R' },new List<char>() {'U','L'},new List<char>() {'U' } } };

    public List<char>[,] mazeB = new List<char>[5, 5] { {new List<char>() { 'R' },new List<char>() { 'L', 'R' ,'D'},new List<char>() { 'L', 'R' },new List<char>() { 'L' },new List<char>() { 'D' } },
        { new List<char>() { 'D' },new List<char>() { 'D','U' }, new List<char>() {'D' },new List<char>() {'D','R' },new List<char>() {'L','U' } },
        {new List<char>() { 'D','U' },new List<char>() {'U','D' },new List<char>() {'U','R','D' },new List<char>() {'L','D','U','R' },new List<char>() {'L' } },
        {new List<char>() {'D','U' },new List<char>() {'U','R' },new List<char>() {'U','L' },new List<char>() {'U','D','R' },new List<char>() {'L','D' } },
        {new List<char>() {'R','U' },new List<char>() {'R','L'},new List<char>() {'R','L' },new List<char>() {'U','L'},new List<char>() {'U' } } };

    public List<char>[,] mazeC = new List<char>[5, 5] { {new List<char>() { 'R','D' },new List<char>() { 'L', 'R' ,'D'},new List<char>() { 'L', 'R' },new List<char>() { 'L', 'R' },new List<char>() { 'L', 'D' } },
        { new List<char>() { 'U' },new List<char>() { 'U' }, new List<char>() {'R','D' },new List<char>() {'R','L' },new List<char>() { 'L', 'U', 'D' } },
        {new List<char>() { 'D','R' },new List<char>() {'R','L' },new List<char>() {'U','L' },new List<char>() {'R','D' },new List<char>() { 'L', 'U', 'D' } },
        {new List<char>() {'U','D' },new List<char>() {'D','R' },new List<char>() {'R','L' },new List<char>() {'U','L' },new List<char>() { 'U' } },
        {new List<char>() {'U' },new List<char>() {'U','R'},new List<char>() {'R','L' },new List<char>() { 'R', 'L' }, new List<char>() {'L' } } };

    public List<char>[,] mazeD = new List<char>[5, 5] { {new List<char>() { 'R','D' },new List<char>() { 'L', 'D'},new List<char>() { 'D', 'R' },new List<char>() { 'L','D', 'R' },new List<char>() { 'L' } },
        { new List<char>() { 'U','D' },new List<char>() { 'U' }, new List<char>() {'U','D' },new List<char>() {'U' },new List<char>() { 'D' } },
        {new List<char>() { 'U','D','R' },new List<char>() {'R','L' },new List<char>() {'L','D','U','R'  },new List<char>() {'R','L' },new List<char>() { 'L', 'U', 'D' } },
        {new List<char>() {'U','R' },new List<char>() {'L' },new List<char>() {'U','D' },new List<char>() {'D' },new List<char>() { 'U' ,'D'} },
        {new List<char>() {'R' },new List<char>() {'L','R'},new List<char>() {'R','L','U' },new List<char>() { 'U', 'L' }, new List<char>() {'U' } } };

    public List<char>[,] mazeE = new List<char>[5, 5] { {new List<char>() { 'R','D' },new List<char>() { 'L', 'D'},new List<char>() { 'D' },new List<char>() { 'D' },new List<char>() { 'D' } },
        { new List<char>() { 'U' },new List<char>() { 'U','D'  }, new List<char>() {'U','D','R' },new List<char>() {'L','D','U','R' },new List<char>() { 'L', 'U', 'D' } },
        {new List<char>() { 'R' },new List<char>() {'U','L','D' },new List<char>() {'D','U'  },new List<char>() {'U' },new List<char>() { 'U' } },
        {new List<char>() {'R' },new List<char>() {'U','L','D' },new List<char>() {'U','R' },new List<char>() {'L', 'R' ,'D' },new List<char>() { 'L' ,'D'} },
        {new List<char>() {'R' },new List<char>() {'R','L','U'},new List<char>() {'R','L' },new List<char>() { 'U', 'L' }, new List<char>() {'U' } } };


    public static List<char>[,] maze = new List<char>[5, 5];

    private List<char> moves = new List<char> { 'U', 'R', 'D', 'L' };
    private List<int> startpos = new List<int>() { 0, 0 };
    private List<int> endpos = new List<int>() { 0, 0 };
    private string solution = "";
    private string input = "";
    private string inputstring = "";
    private int initialfontsize = 0;
    private int indexoffset = 0;
    private int mazeselect = 0;
    private bool overload = false;


    private void Awake()
    {
        moduleId = moduleIdCounter++;
        foreach (KMSelectable button in buttons)
        {
            KMSelectable pressedButton = button;
            button.OnInteract += delegate () { ButtonPress(pressedButton); return false; };
        }
    }


    // Use this for initialization
    void Start()
    {
        //Generate starting string
        inputstring = GenerateStartingString();
        ScreenText.text = inputstring;
        Debug.LogFormat("[Look And Say #{0}] The starting string is: " + inputstring, moduleId);

        //Used for editing font size in ButtonPress
        initialfontsize = ScreenText.fontSize;

        //Find maze and offset
        indexoffset = mod(ConvToPosSmall(bomb.GetSerialNumber()[1]) + (int)char.GetNumericValue(inputstring[11]),5);
        mazeselect = mod(ConvToPosSmall(bomb.GetSerialNumber()[0]) + (int)char.GetNumericValue(inputstring[0]),5);
        ChooseMaze(mazeselect);
        Debug.LogFormat("[Look And Say #{0}] Use maze " + mazeselect, moduleId);
        Debug.LogFormat("[Look And Say #{0}] The offset is " + indexoffset, moduleId);

        //Convert using Look and say 
        string lookandsaystring = GetLookAndSay(inputstring);
        Debug.LogFormat("[Look And Say #{0}] The string after converting is: " + lookandsaystring, moduleId);

        //Convert string to moves
        List<char> movelist = ConvertStringToMoves(lookandsaystring);
        string movesstring = ListToString(movelist);
        Debug.LogFormat("[Look And Say #{0}] Converting the string to moves: " + movesstring, moduleId);
        
        //Find start position
        startpos = new List<int>() { mod(ConvToPosSmall(bomb.GetSerialNumber()[2])+ (int)char.GetNumericValue(inputstring[1]),5), mod(ConvToPosSmall(bomb.GetSerialNumber()[3]) + (int)char.GetNumericValue(inputstring[2]), 5) };
        Debug.LogFormat("[Look And Say #{0}] Starting position is: (" + startpos[0].ToString() + "," + startpos[1].ToString() + ")",moduleId);

        //Shift start position using moves
        ShiftStartPos(startpos, movelist);
        Debug.LogFormat("[Look And Say #{0}] Moved starting position to: (" + startpos[0].ToString() + "," + startpos[1].ToString() + ")",moduleId);

        //Find end position
        endpos = new List<int>() { mod(ConvToPosSmall(bomb.GetSerialNumber()[4]) + (int)char.GetNumericValue(inputstring[9]), 5), mod(ConvToPosSmall(bomb.GetSerialNumber()[5]) + (int)char.GetNumericValue(inputstring[10]), 5) };
        Debug.LogFormat("[Look And Say #{0}] Ending position is: (" + endpos[0].ToString() + "," + endpos[1].ToString() + ")",moduleId);

        //If shifted position is the same as the end position then use the original start position->0,0->4,4
        if (startpos.SequenceEqual(endpos))
        {
            startpos = new List<int>() { mod(ConvToPosSmall(bomb.GetSerialNumber()[2]) + (int)char.GetNumericValue(inputstring[1]), 5), mod(ConvToPosSmall(bomb.GetSerialNumber()[3]) + (int)char.GetNumericValue(inputstring[2]), 5) };
            Debug.LogFormat("[Look And Say #{0}] Starting position changed to: (" + startpos[0].ToString() + "," + startpos[1].ToString() + ")", moduleId);
        }
        if (startpos.SequenceEqual(endpos))
        {
            startpos = new List<int>() { 0, 0 };
            Debug.LogFormat("[Look And Say #{0}] Starting position changed to: (" + startpos[0].ToString() + "," + startpos[1].ToString() + ")", moduleId);
        }
        if (startpos.SequenceEqual(endpos))
        {
            startpos = new List<int>() { 4, 4 };
            Debug.LogFormat("[Look And Say #{0}] Starting position changed to: (" + startpos[0].ToString() + "," + startpos[1].ToString() + ")", moduleId);
        }

        //Find shortest path from start to end
        movelist = new List<char>(FindPath(startpos, endpos));
        if (overload) return;
        Debug.LogFormat("[Look And Say #{0}] Shortest path is: " + ListToString(movelist),moduleId);

        //Convert moves back to numbers
        StringBuilder builder = new StringBuilder();
        List<int> movelistvalues = new List<int>();
        movelistvalues = ConvertMovesToValues(movelist);
        foreach (int i in movelistvalues) builder.Append(i);
        movesstring = builder.ToString();
        Debug.LogFormat("[Look And Say #{0}] The moves converted to values: " + movesstring,moduleId);

        //Revert look and say 
        solution = GetInvertedLookAndSay(movesstring);
        Debug.LogFormat("[Look And Say #{0}] The solution is: " + solution,moduleId);
        moduleReady = true;
    }

    void ButtonPress(KMSelectable button)
    {
        bombaudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, this.transform);
        button.AddInteractionPunch();
        if (moduleReady && !overload)
        {
            if (button.name != "KeyReset") ScreenText.text = "";
            switch (button.name)
            {
                case "Key1":
                    if (input.ToArray().Length == solution.ToArray().Length)
                    {
                        module.HandleStrike();
                        ScreenText.text = input; 
                        break;
                    }
                    input += "1";
                    ScreenText.text += input;
                    break;
                case "Key2":
                    if (input.ToArray().Length == solution.ToArray().Length)
                    {
                        module.HandleStrike();
                        ScreenText.text = input;
                        break;
                    }
                    input += "2";
                    ScreenText.text += input;
                    break;
                case "Key3":
                    if (input.ToArray().Length == solution.ToArray().Length)
                    {
                        module.HandleStrike();
                        ScreenText.text = input;
                        break;
                    }
                    input += "3";
                    ScreenText.text += input;
                    break;
                case "Key4":
                    if (input.ToArray().Length == solution.ToArray().Length)
                    {
                        module.HandleStrike();
                        ScreenText.text = input;
                        break;
                    }
                    input += "4";
                    ScreenText.text += input;
                    break;
                case "KeySubmit":
                    Debug.LogFormat("[Look And Say #{0}] User submitted: " + input,moduleId);
                    if (input == solution)
                    {
                        bombaudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, this.transform);
                        Debug.LogFormat("[Look And Say #{0}] Module solved!", moduleId);
                        ScreenText.fontSize = initialfontsize;
                        ScreenText.text = "SOLVED";
                        module.HandlePass();
                        moduleReady = false;
                    }
                    else
                    {
                        Debug.LogFormat("[Look And Say #{0}] Incorrect sequence submitted! Strike!", moduleId);
                        module.HandleStrike();
                        input = "";
                    }
                    break;
                case "KeyReset":
                    Debug.LogFormat("[Look And Say #{0}] User pressed reset.", moduleId);
                    input = "";
                    ScreenText.text = inputstring;
                    ScreenText.fontSize = initialfontsize;
                    break;
            }
            
            if (ScreenText.text.Length > 12) ScreenText.fontSize = (int)(initialfontsize*Math.Pow(0.93,(double)(ScreenText.text.Length - 12)));
        }
        else if (overload)
        {
            bombaudio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime, this.transform);
            Debug.LogFormat("[Look And Say #{0}] Module solved? I guess we will never know for sure...", moduleId);
            ScreenText.fontSize = initialfontsize;
            ScreenText.text = "SOLVED?";
            module.HandlePass();
            moduleReady = false;
        }
    }

    private string GenerateStartingString()
    {
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < 12; i++)
        {
            builder.Append(UnityEngine.Random.Range(1, 5));
        }

        return builder.ToString();
    }

    private class Path
    {
        //initialize local variables
        private List<int> start = new List<int>();
        private List<int> end = new List<int>();
        private List<List<int>> cellhistory = new List<List<int>>();
        private List<char> moves = new List<char>();
        private bool pathfinished = false;

        //constructor
        public Path(List<int> _start, List<int> _end)
        {
            start = new List<int>(_start);
            end = new List<int>(_end);
            cellhistory.Add(start);
        }

        //Apply a move and add the cell to the cell history
        //The cellhistory ensures paths don't cross themselves
        public void Move(char move)
        {
            List<int> newcell = new List<int>(CurrentCell());
            if (move == 'U') newcell[0] = CurrentCell()[0] - 1;
            if (move == 'R') newcell[1] = CurrentCell()[1] + 1;
            if (move == 'D') newcell[0] = CurrentCell()[0] + 1;
            if (move == 'L') newcell[1] = CurrentCell()[1] - 1;
            moves.Add(move);
            cellhistory.Add(newcell);
        }

        //Returns the cell the path is currently located at
        public List<int> CurrentCell()
        {
            return cellhistory.Last();
        }

        //Returns the list of moves the path took
        public List<char> GetMoveList()
        {
            return moves;
        }

        //Main algorithm of the path 
        public bool GetPath()
        {
            //Initialize variables
            List<char> possiblemoves = new List<char>();
            List<int> fork = new List<int>();
            List<char> movesatfork = new List<char>();
            List<int> testmove = new List<int>();
            bool forkupdated = false;
            List<char> shortmoves = new List<char>();
            List<char> longmoves = new List<char>();
            List<char> reorderedmoves = new List<char>();

            //Path continues searching until it reaches the end, or gets stuck
            while (!pathfinished)
            {
                //Get the list of possible moves from the current cell of the maze
                //Initialize move lists
                possiblemoves = new List<char>(maze[CurrentCell()[0], CurrentCell()[1]]);
                shortmoves.Clear();
                longmoves.Clear();
                reorderedmoves.Clear();

                //Loop over the possible moves that can be taken from the current cell
                foreach (char move in possiblemoves)
                {
                    //Take a temporary step in the current move direction
                    //Calculate the distance in the row and column dimensions between the test cell and the end
                    //Ideally we are always going to make moves that take us closer to the end in one of the two dimensions
                    testmove = MoveTest(CurrentCell(), move);
                    if (!ListContainsList(cellhistory, testmove))
                    {
                        if (Distance(testmove, end)[0] < Distance(CurrentCell(), end)[0] || Distance(testmove, end)[1] < Distance(CurrentCell(), end)[1])
                        {
                            shortmoves.Add(move); //maintain list of moves which decrease the distance
                        }
                        else
                        {
                            longmoves.Add(move); //maintain the list of moves which increase the distance 
                        }
                    }
                }

                //We order the moves starting with short moves then long moves. This might(?) make the algorithm more efficient
                reorderedmoves = new List<char>(shortmoves);
                reorderedmoves.AddRange(longmoves);
                if (reorderedmoves.Count == 0) pathfinished = true; //If the list of short + long moves is empty then the path ended
                else
                {
                    //Otherwise we need to do two important things for pathing. First, we need to maintain the list of moves at the last "fork"
                    //A fork is a cell where there are multiple possible moves to take, but we don't know what the last fork will be.
                    //We just keep overwriting the forks, so that the final state of the fork is the last one for the path. 
                    //Then we make the first move in the list, this means taking a short moves first if possible
                    if (reorderedmoves.Count > 1)
                    {
                        fork = new List<int>(CurrentCell());
                        movesatfork = new List<char>(reorderedmoves);
                        forkupdated = true;
                    }
                    Move(reorderedmoves[0]);
                    if (CurrentCell().SequenceEqual(end)) pathfinished = true; //After the move, check if the path reached the end
                }
            }

            //If there was ever a fork then we remove the first element of the forks move list, and update the maze for that cell. 
            //This ensures that for the last fork, that move is never taken again by another path to prevent copies.
            if (forkupdated)
            {
                movesatfork.RemoveAt(0);
                maze[fork[0], fork[1]] = new List<char>(movesatfork);
            }
            return CurrentCell().SequenceEqual(end); //The return of GetPath is true if the path reached the end

        }

    }

    //This is a way to compare a cells coordinates, to a pair of coordinates in a list, kinda hacky but it works
    public static bool ListContainsList(List<List<int>> lists, List<int> l)
    {
        foreach (List<int> list in lists)
        {
            if (list.SequenceEqual(l)) return true;
        }
        return false;
    }

    //Returns a pseudo-vector, where x is the distance between the two cells in the x-direction and y for the y-direction
    public static List<int> Distance(List<int> cellA, List<int> cellB)
    {
        return new List<int>() { Math.Abs(cellA[0] - cellB[0]), Math.Abs(cellA[1] - cellB[1]) };
    }

    //Make a temporary move, and return the new cell
    public static List<int> MoveTest(List<int> cell, char move)
    {
        List<int> newcell = new List<int>(cell);
        if (move == 'U') newcell[0] = cell[0] - 1;
        if (move == 'R') newcell[1] = cell[1] + 1;
        if (move == 'D') newcell[0] = cell[0] + 1;
        if (move == 'L') newcell[1] = cell[1] - 1;
        return newcell;
    }

    //This is the method that generates paths
    public List<char> FindPath(List<int> start, List<int> end)
    {
        int pathcount = 0;
        Path p = new Path(start, end); //Tells the path the start and end goal
        while (!p.GetPath()) //Keep generating paths until the path reaches the goal
        {
            p = new Path(start, end);
            pathcount++;
            if (pathcount > 300) //Again just to be safe for potentially infinite loops
            {
                Debug.LogFormat("[Look And Say #{0}] Too many paths generated... overloading.", moduleId);
                ScreenText.text = "OVERLOAD";
                overload = true;
                moduleReady = true; 
                break;
            }
        }
        return p.GetMoveList(); //Returns the move list for the shortest path from start to end       
    }

    //Moves the start position from the pre-determined start using the moves decoded on the module
    void ShiftStartPos(List<int> startpos, List<char> movelist)
    {
        int countUD = 0;
        int countLR = 0;

        foreach (char c in movelist)
        {
            if (c == 'U') countUD--;
            if (c == 'D') countUD++;
            if (c == 'L') countLR--;
            if (c == 'R') countLR++;
        }

        startpos[0] = mod((startpos[0] + countUD), 5);
        startpos[1] = mod((startpos[1] + countLR), 5);
    }

    //Returns the next step of the Look and Say sequence for the current string
    string GetLookAndSay(string s)
    {
        s += "e"; //finish character to make sure output string collects all digits
        char[] arr = s.ToCharArray(); //convert string to array 

        //initialize variables
        char currentchar = 'a';
        char previouschar = 'a';
        var substringsize = 0;
        string output = "";

        //collect characters and lengths of consecutive characters to generate output string
        for (int i = 0; i < arr.Length; i++)
        {
            currentchar = arr[i];
            if (currentchar != previouschar && i != 0)
            {
                output += substringsize.ToString() + previouschar;
                substringsize = 0;
            }
            substringsize++;
            previouschar = arr[i];
        }

        return output;
    }

    //Reverses the Look and Say process
    string GetInvertedLookAndSay(string s)
    {
        List<char> arr = s.ToList<char>();

        //initialize variables
        int length = 0;
        char value = 'a';
        string output = "";

        //SPECIAL CASE: if arr.Length is odd then append 1 (or maybe append special digit based on bomb) 
        if (arr.Count % 2 == 1) arr.Add('1'); //TODO: Decide if we want this number to depend on bomb info

        //Take each pair of characters, first is length, second is value, and append value to the string length times
        for (int i = 0; i < arr.Count; i += 2)
        {
            length = (int)char.GetNumericValue(arr[i]);
            value = arr[i + 1];
            for (int j = 0; j < length; j++) output += value;
        }

        return output;
    }

    //Converts the digits to moves
    List<char> ConvertStringToMoves(string s)
    {
        char[] arr = s.ToCharArray();

        //initialize variables
        List<char> output = new List<char>();
        int index = 0;

        //loop through elements of array to convert numbers to moves and append to List 
        for (int i = 0; i < arr.Length; i++)
        {
            index = (int)char.GetNumericValue(arr[i]) - 1; //base 0
            index += indexoffset; 
            index %= 4;
            output.Add(moves[index]);
        }

        return output;
    }

    //Takes a list of moves and converts them to a list of integers
    List<int> ConvertMovesToValues(List<char> l)
    {
        List<int> movevalues = new List<int>();
        foreach (char c in l)
        {
            movevalues.Add(mod((moves.IndexOf(c) - indexoffset),4) + 1);
        }

        return movevalues;
    }

    private void ChooseMaze(int i)
    {
        switch (i)
        {
            case 0:
                maze = (List<char>[,]) mazeA.Clone();
                break;
            case 1:
                maze = (List<char>[,]) mazeB.Clone();
                break;
            case 2:
                maze = (List<char>[,]) mazeC.Clone();
                break;
            case 3:
                maze = (List<char>[,]) mazeD.Clone();
                break;
            case 4:
                maze = (List<char>[,]) mazeE.Clone();
                break;
        }
    }

    //Hacky method to convert serial characters to row,col mod 5
    private int ConvToPosSmall(char serialelement)
    {
        int num = serialelement - '0';
        if (num > 9)
        {
            num += '0' - 'A' + 1;
        }
        while (num > 9)
        {
            num -= 10;
        }
        while (num > 4)
        {
            num -= 5;
        }
        return num;
    }

    //String methods, just for Logging
    private static string ListToString(List<string> l)
    {
        string str = "";
        foreach (var s in l) str += s;
        return str;
    }

    private static string ListToString(List<int> l)
    {
        string str = "";
        foreach (var s in l) str += s.ToString();
        return str;
    }

    private static string ListToString(List<char> l)
    {
        string str = "";
        foreach (var s in l) str += s.ToString();
        return str;
    }

    private int mod(int x, int m)
    {
        return (x % m + m) % m;
    }

    //twitch plays
    private bool inputIsValid(string cmd)
    {
        string[] validstuff = { "1", "2", "3", "4", "submit", "reset" };
        if (validstuff.Contains(cmd.ToLower()))
        {
            return true;
        }
        return false;
    }

#pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} press <1/2/3/4/submit/reset> [Presses the specified button]. You can also string presses together i.e. press 1 1 2 3, press 1,1,2,3";
#pragma warning restore 414
    IEnumerator ProcessTwitchCommand(string command)
    {
        string[] parameters = command.Split(' ', ',');
        if (Regex.IsMatch(parameters[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            for (int i = 1; i < parameters.Length; i++)
            {
                if (inputIsValid(parameters[i]))
                {
                    yield return null;
                    if (parameters[i].ToLower().Equals("1"))
                    {
                        buttons[0].OnInteract();
                    }
                    else if (parameters[i].ToLower().Equals("2"))
                    {
                        buttons[1].OnInteract();
                    }
                    else if (parameters[i].ToLower().Equals("3"))
                    {
                        buttons[2].OnInteract();
                    }
                    else if (parameters[i].ToLower().Equals("4"))
                    {
                        buttons[3].OnInteract();
                    }
                    else if (parameters[i].ToLower().Equals("submit"))
                    {
                        buttons[4].OnInteract();
                    }
                    else if (parameters[i].ToLower().Equals("reset"))
                    {
                        buttons[5].OnInteract();
                    }
                }
            }
            yield break;
        }
    }

}


