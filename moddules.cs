using System;
using System.Collections.Generic;
using System.Text;

namespace autocad_part2
{
    public class Abc
    {



        /**************** model ********************/

        static void Main(string[] args)
        {
            if (abc2svg.loadjs == null)
            {
                abc2svg.loadjs = (fn, onsuccess, onerror) =>
                {
                    onerror?.Invoke(fn);
                };
            }

            abc2svg.modules = new
            {
                ambitus = new { },
                begingrid = new { fn = "grid3" },
                beginps = new { fn = "psvg" },
                @break = new { },
                capo = new { },
                chordnames = new { },
                clip = new { },
                clairnote = new { fn = "clair" },
                voicecombine = new { fn = "combine" },
                diagram = new { fn = "diag" },
                equalbars = new { },
                gamelan = new { },
                grid = new { },
                grid2 = new { },
                jazzchord = new { },
                jianpu = new { },
                mdnn = new { },
                MIDI = new { },
                nns = new { },
                pageheight = new { fn = "page" },
                pedline = new { },
                percmap = new { fn = "perc" },
                roman = new { },
                soloffs = new { },
                sth = new { },
                strtab = new { },
                temperament = new { fn = "temper" },
                tropt = new { },

                nreq = 0,

                // scan the file and find the required modules
                // @file: ABC file
                // @relay: (optional) callback function for continuing the treatment
                // @errmsg: (optional) function to display an error message if any
                // This function gets one argument: the message
                // return true when all modules are loaded
                load = (Func<string, Action, Action<string>, bool>)((file, relay, errmsg) =>
                {
                    Func<Action<string>> get_errmsg = () =>
                    {
                        if (user is IDictionary<string, object> userDict && userDict.ContainsKey("errmsg"))
                            return user.errmsg;
                        if (abc2svg.printErr != null)
                            return abc2svg.printErr;
                        if (typeof(Action<string>) == typeof(Action<string>))
                            return m => Console.WriteLine(m);
                        return m => { };
                    };

                    Action load_end = () =>
                    {
                        if (--abc2svg.modules.nreq == 0)
                            abc2svg.modules.cbf();
                    };

                    var m, i, fn;
                    int nreq_i = abc2svg.modules.nreq;
                    var ls = System.Text.RegularExpressions.Regex.Matches(file, @"(^|\n)(%%|I:).+?\b");

                    if (ls.Count == 0)
                        return true;

                    abc2svg.modules.cbf = relay ?? (() => { });
                    abc2svg.modules.errmsg = errmsg ?? get_errmsg();

                    foreach (System.Text.RegularExpressions.Match match in ls)
                    {
                        string fn = match.Value.Replace("\n?(%%|I:)", "");
                        var m = abc2svg.modules[fn];
                        if (m == null || m.loaded)
                            continue;

                        m.loaded = true;

                        if (m.fn != null)
                            fn = m.fn;
                        abc2svg.modules.nreq++;
                        abc2svg.loadjs(fn + "-1.js",
                            load_end,
                            () =>
                            {
                                abc2svg.modules.errmsg($"Error loading the module {fn}");
                                load_end();
                            });
                    }
                    return abc2svg.modules.nreq == nreq_i;
                })
            };
        }

        static void EmptyFunction() { }




    }
}
