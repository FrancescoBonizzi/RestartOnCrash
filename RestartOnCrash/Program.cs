using RestartOnCrash.UI;

using System;
using System.Windows.Forms;

namespace RestartOnCrash;

/// <summary>
/// Main class
/// </summary>
public class Program
{

    [STAThread]
    static void Main(string[] args)
    {
        Application.Run(new MainForm());
    }
}