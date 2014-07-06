using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
//using System.Threading.Tasks;

//  Name      : Ibrahim Al-Alali
//  Student ID: 20092171051
//  Instructor: Dr. Inad Aljarrah
//  [24/2/2013]  
//  To use this application just call the function "TheGame.Start()" in the Main
//  and the application will start automagically
//

namespace The_8_Puzzle_Uninformed {
  
    
        enum NodePosition {
            UPPER_LEFT = 0,
            UPPER,
            UPPER_RIGHT,
            LEFT,
            MIDDLE,
            RIGHT,
            LOWER_LEFT,
            LOWER,
            LOWER_RIGHT
        }
        enum Actions {
            Up = 0,
            Down = 1,
            Left = 2,
            Right = 3,
            NoMovement = 4
        }
        class State {
            public NodePosition Position { get; private set; }
            public State parent { get; private set; }
            public int[] theGame;
            public Actions Move { get; set; }
    
            public State( State parent, int[] theGame, NodePosition position, Actions move ) {
                this.parent = parent;
                this.Position = position;
                this.theGame = new int[9];
                theGame.CopyTo( this.theGame, 0 );
                Move = move;
            }
    
            public override string ToString() {
                string s = string.Empty;
                foreach ( int i in theGame ) {
                    s += i;
                }
                return s + ", " + Position + ", " + Move;
            }
        }
    
        static class TheGame {
            static bool isUnique( string input ) {
                for ( int i = 0; i < input.Length - 1; i++ ) {
                    for ( int j = i + 1; j < input.Length; j++ ) {
                        if ( input[i] != input[j] ) {
                            continue;
                        } else {
                            return false;
                        }
                    }
                }
                return true;
            }
            static private string printThe8Puzzle( int[] theGame ) {
                string Write = string.Empty;
                Write += "\n\t";
                for ( int i = 0; i < theGame.Length; i++ ) {
                    if ( theGame[i] == 0 ) {
                        Write += "  ";
                    } else {
                        Write += theGame[i] + " ";
                    }
    
                    if ( ( i + 1 ) % 3 == 0 && i != 0 ) {
                        Write += "\n\t";
                    }
                }
                Write += Environment.NewLine;
    
                return Write;
            }
            static private void PrintToConsole( int[] theGame ) {
                Console.WriteLine( printThe8Puzzle( theGame ) );
            }
            static private void PrintTheMovement( List<State> queue ) {
                int j = 0;
                List<State> printList = new List<State>();
                printList.Add( queue.Last() );
    
                while ( printList.Last().parent.Move != Actions.NoMovement ) {
                    printList.Add( printList.Last().parent );
                }
                printList.Reverse();
    
                Console.Write( "\n\t" );
                foreach ( State s in printList ) {
                    Console.Write( s.Move );
                    if ( !isGoal( s.theGame ) ) {
                        Console.Write( ", " );
                    }
                    if ( ( j + 1 ) % 5 == 0 ) {
                        Console.Write( "\n\t" );
                    }
                    j++;
                }
                Console.WriteLine( "\n\n\tNumber Movements: " + j );
            }
            private static NodePosition getWhereToGO( NodePosition Position, Actions whereToGo ) {
    
                switch ( whereToGo ) {
                    case Actions.Up:
                        switch ( Position ) {
                            case NodePosition.LEFT: return NodePosition.UPPER_LEFT;
                            case NodePosition.MIDDLE: return NodePosition.UPPER;
                            case NodePosition.RIGHT: return NodePosition.UPPER_RIGHT;
                            case NodePosition.LOWER_LEFT: return NodePosition.LEFT;
                            case NodePosition.LOWER: return NodePosition.MIDDLE;
                            case NodePosition.LOWER_RIGHT: return NodePosition.RIGHT;
                            default: return Position;
                        }
                    case Actions.Down:
                        switch ( Position ) {
                            case NodePosition.UPPER_LEFT: return NodePosition.LEFT;
                            case NodePosition.UPPER: return NodePosition.MIDDLE;
                            case NodePosition.UPPER_RIGHT: return NodePosition.RIGHT;
                            case NodePosition.LEFT: return NodePosition.LOWER_LEFT;
                            case NodePosition.MIDDLE: return NodePosition.LOWER;
                            case NodePosition.RIGHT: return NodePosition.LOWER_RIGHT;
                            default: return Position;
                        }
    
                    case Actions.Left:
                        switch ( Position ) {
                            case NodePosition.UPPER: return NodePosition.UPPER_LEFT;
                            case NodePosition.UPPER_RIGHT: return NodePosition.UPPER;
                            case NodePosition.MIDDLE: return NodePosition.LEFT;
                            case NodePosition.RIGHT: return NodePosition.MIDDLE;
                            case NodePosition.LOWER: return NodePosition.LOWER_LEFT;
                            case NodePosition.LOWER_RIGHT: return NodePosition.LOWER;
                            default: return Position;
                        }
                    case Actions.Right:
                        switch ( Position ) {
                            case NodePosition.UPPER_LEFT: return NodePosition.UPPER;
                            case NodePosition.UPPER: return NodePosition.UPPER_RIGHT;
                            case NodePosition.LEFT: return NodePosition.MIDDLE;
                            case NodePosition.MIDDLE: return NodePosition.RIGHT;
                            case NodePosition.LOWER_LEFT: return NodePosition.LOWER;
                            case NodePosition.LOWER: return NodePosition.LOWER_RIGHT;
                            default: return Position;
                        }
                    default: return Position;
                }
    
    
            }
            private static void MoveNodes( ref int[] theGame, NodePosition position1, NodePosition position2 ) {
                int temp = theGame[( int )position1];
                theGame[( int )position1] = theGame[( int )position2];
                theGame[( int )position2] = temp;
            }
            private static bool isGoal( int[] theGame ) {
                for ( int i = 0; i < theGame.Length; i++ ) {
                    if ( theGame[i] != i ) {
                        return false;
                    }
                }
                return true;
            }
            private static bool isRepeated( List<State> queue, int[] OtherGame ) {
                for ( int i = 0; i < queue.Count - 1; i++ ) {
                    if ( isEqual( queue[i].theGame, OtherGame ) ) {
                        return true;
                    }
                }    
                return false;
            }
            private static bool isEqual( int[] list1, int[] list2 ) {
                // list1 & list2 are always equal in length
                for ( int i = 0; i < list1.Length; i++ ) {
                    if ( list1[i] != list2[i] ) {
                        return false;
                    }
                }
                return true;
            }
    
            public static void Start() {
                Console.Title = "The 8 Puzzle - Uninformed Edition";
    
                do {
                    #region Initialization
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine( "\t\t\tThe 8 Puzzle - Uninformed Edition\n" );
                    Console.ResetColor();
                    List<State> queue = new List<State>();
                    int expantionNode = 0;
                    int index = 0;
                    int position = 0;
                    int generatedNodes = 0;
                    int expandedState = 0;
                    int[] theGame = new int[9];
                    int[] tempGame = new int[9];
                    bool noSolution = true;
                    StringBuilder cInput;
                    string input;
                    Console.WriteLine( "\tPress Control + C at any time to cancel and exit the application" );
                    #endregion
                    #region Checking Code
                    Console.WriteLine( "\tPlease Enter the numbers for the starting position:" );
                    Console.Write( '\t' );
                    input = Console.ReadLine();
                    if ( input.Length != 9 ) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine( "\tPlease Enter 8 numbers and one \'S\'\n" );
                        Console.ResetColor();
                        goto ClearScreen;
                    }
                    if ( !isUnique( input ) ) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine( "\tThe numbers must be unique, and only ons \'S\' is allowed\n" );
                        Console.ResetColor();
                        goto ClearScreen;
                    }
                    cInput = new StringBuilder( input );
                    bool areNumbers = false;
                    cInput.Replace( 'S', '0' );
                    cInput.Replace( 's', '0' );
                    int temp;
                    bool error = false;
    
                    // try the parallel for loop to speed up the process
    
    
                    for ( int i = 0; i < cInput.Length; i++ ) {
                        if ( int.TryParse( cInput[i].ToString(), out temp ) ) {
                            if ( temp == 9 ) {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine( "\tThe number 9 is not allowed, only from 1 to 8" );
                                Console.ResetColor();
                                error = true;
                                break;
                            } else if ( temp == 0 ) {
                                // get the position of the blank space
                                index = i;
                            } else {
                                // copying the array of characters to an array of integers
                                theGame[i] = temp;
                            }
    
                            areNumbers = true;
                            continue;
                        }
                        areNumbers = false;
                        break;
                    }
    
                    if ( error ) {
                        goto ClearScreen;
                    }
                    if ( !areNumbers ) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine( "\tNo letters are allowed except for the letter \'S\'\n" );
                        Console.ResetColor();
                        goto ClearScreen;
                    }
                    #endregion
    
                    PrintToConsole( theGame );
    
                    // start the stopwatch
                    Stopwatch s = new Stopwatch();
                    s.Start();
    
                    // Added the root here
                    queue.Add( new State( null, theGame, ( NodePosition )index, Actions.NoMovement ) );
                    generatedNodes++;
                    if ( isGoal( queue[expantionNode].theGame ) ) {
                        noSolution = false;
                        // do the end of the game
                        goto TheEnd;
                    }
    
                    #region State expansion
                    do {
                        expandedState++;
                        queue[expantionNode].theGame.CopyTo( tempGame, 0 );
    
                        #region Expansion Code
                        position = ( int )getWhereToGO( queue[expantionNode].Position, Actions.Up );
                        if ( position != ( int )queue[expantionNode].Position ) {
                            MoveNodes( ref tempGame, queue[expantionNode].Position, ( NodePosition )position );
                            queue.Add( new State( queue[expantionNode], tempGame, ( NodePosition )position, Actions.Up ) );
                            if ( isGoal( queue.Last().theGame ) ) {
                                noSolution = false;
                                break;
                            }
                            // removes the repeated state
                            if ( isRepeated( queue, queue.Last().theGame ) ) {
                                queue.RemoveAt( queue.Count - 1 );
                            } else {
                                generatedNodes++;
                            }
                            queue[expantionNode].theGame.CopyTo( tempGame, 0 );
                        }
    
                        position = ( int )getWhereToGO( queue[expantionNode].Position, Actions.Down );
                        if ( position != ( int )queue[expantionNode].Position ) {
                            MoveNodes( ref tempGame, queue[expantionNode].Position, ( NodePosition )position );
                            queue.Add( new State( queue[expantionNode], tempGame, ( NodePosition )position, Actions.Down ) );
                            if ( isGoal( queue.Last().theGame ) ) {
                                noSolution = false;
                                break;
                            }
                            // removes the repeated state
                            if ( isRepeated( queue, queue.Last().theGame ) ) {
                                queue.RemoveAt( queue.Count - 1 );
                            } else {
                                generatedNodes++;
                            }
                            queue[expantionNode].theGame.CopyTo( tempGame, 0 );
                        }
    
                        position = ( int )getWhereToGO( queue[expantionNode].Position, Actions.Left );
                        if ( position != ( int )queue[expantionNode].Position ) {
                            MoveNodes( ref tempGame, queue[expantionNode].Position, ( NodePosition )position );
                            queue.Add( new State( queue[expantionNode], tempGame, ( NodePosition )position, Actions.Left ) );
                            if ( isGoal( queue.Last().theGame ) ) {
                                noSolution = false;
                                break;
                            }
                            // removes the repeated state
                            if ( isRepeated( queue, queue.Last().theGame ) ) {
                                queue.RemoveAt( queue.Count - 1 );
                            } else {
                                generatedNodes++;
                            }
                            queue[expantionNode].theGame.CopyTo( tempGame, 0 );
                        }
    
                        position = ( int )getWhereToGO( queue[expantionNode].Position, Actions.Right );
                        if ( position != ( int )queue[expantionNode].Position ) {
                            MoveNodes( ref tempGame, queue[expantionNode].Position, ( NodePosition )position );
                            queue.Add( new State( queue[expantionNode], tempGame, ( NodePosition )position, Actions.Right ) );
                            if ( isGoal( queue.Last().theGame ) ) {
                                noSolution = false;
                                break;
                            }
                            // removes the repeated state
                            if ( isRepeated( queue, queue.Last().theGame ) ) {
                                queue.RemoveAt( queue.Count - 1 );
                            } else {
                                generatedNodes++;
                            }
                            queue[expantionNode].theGame.CopyTo( tempGame, 0 );
                        }
    
                        #endregion
    
                        expantionNode++;
                    } while ( queue.Count != expantionNode );
                    #endregion
    
                TheEnd:
                    
                if ( noSolution ) {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine( "\n\t!! No solution found !!\n" );
                    }
                    
                    // stop the stopwatch
                    s.Stop();
                    TimeSpan timeSpan = s.Elapsed;
    
                    #region End of application
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine( "\n\tFinished" );
                    Console.ResetColor();
    
                    Console.Write( "\tGenerated Nodes: " );
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write( generatedNodes.ToString() + "\n" );
                    Console.ResetColor();
    
                    Console.Write( "\tExpanded States: " );
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write( expandedState.ToString() + "\n" );
                    Console.ResetColor();
                    
                    Console.Write( "\tElapsed Time: " );
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write( timeSpan.ToString() + "\n" );
                    Console.ResetColor();
    
                    if ( !noSolution ) {
                        Console.WriteLine( "\tThe Solution:" );
                        PrintToConsole( queue.Last().theGame );
                        Console.WriteLine( Environment.NewLine );
                        PrintTheMovement( queue );
                    }
    
                ClearScreen:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine( "\n\tPress any key to Continue" );
                    Console.ResetColor();
                    Console.ReadKey( true );
                    Console.Clear();
                    #endregion
    
                } while ( true );
            }
        }
    
        class Program {
            static void Main( string[] args ) {
                TheGame.Start();
            }
        }
  



    
}