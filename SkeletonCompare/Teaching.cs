using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace SkeletonCompare
{
    public class Teaching
    {
        private string path = "..\\..\\..\\ReferenceData\\";
        private List<string> exerciseNamesRef;
        private List<Skeleton> Skeletons;
        public Teaching(string exerciseNameRef)
        {
            if (!File.Exists(path + exerciseNameRef + "Average.txt")) {
                exerciseNamesRef = new List<string>();
                Skeletons = new List<Skeleton>();
                if (File.Exists(path + exerciseNameRef + ".txt"))
                {
                    exerciseNamesRef.Add(exerciseNameRef + ".txt");

                    for (int i = 1; ; ++i)
                    {
                        if (File.Exists(path + exerciseNameRef + i + ".txt"))
                        {
                            exerciseNamesRef.Add(exerciseNameRef + i + ".txt");
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                Average();
            }
        }

        public void Average()
        {
            string[] names = exerciseNamesRef[0].Split('.');
            //StreamWriter streamWriter = new StreamWriter(path + names[0] + "Avarage.txt");
            System.IO.File.WriteAllText(path + names[0] + "Average.txt", "0 0 0 0 0 JointType     DepthPointX        DepthPointY         WidthOfDisplay  HeightOfDisplay" + Environment.NewLine);
            string width = "";
            string height = "";
            List<string> line = new List<string>(); // A line in the file
            List<string[]> words = new List<string[]>();
            char[] separators = { ' ' };

            List<StreamReader> file = new List<StreamReader>();
            foreach (string fileName in exerciseNamesRef)
            {
                if (File.Exists(path + fileName))
                {
                    file.Add(new StreamReader(path + fileName));
                    //read the first line, because it is the bill head and this not need it
                    string firstLine = file[file.Count - 1].ReadLine();
                    line.Add(null);
                    words.Add(null);
                }
                else
                {
                    return;
                }

            }


            //read the skeleton data
            while (true)
            {
                int jointNumber = 25;
                int counter = 0;
                Vector3D sum = new Vector3D(0, 0, 0); // the coordinates sum
                for (int i = 0; i < file.Count(); ++i)
                {
                    if (line[i] == null)
                    {
                        line[i] = file[i].ReadLine();
                        // split the data
                        words[i] = line[i].Split(separators);
                        //if the line is a first line
                        double number;
                        if (!Double.TryParse(words[i][0], out number))
                        {
                            break;
                        }
                        width = words[i][10];
                        height = words[i][11];
                    }

                    foreach (string[] w in words)
                    {
                        if (w != null) {
                            if (Convert.ToInt32(w[5]) < jointNumber)
                            {
                                jointNumber = Convert.ToInt32(w[5]);
                            }
                        }
                    }


                    if (Convert.ToInt32(words[i][5]) == jointNumber)
                    {
                        sum.X += Convert.ToDouble(words[i][8]);//depth point
                        sum.Y += Convert.ToDouble(words[i][9]);//depth point

                        line[i] = null;
                        words[i] = null;
                        counter++;
                    }

                    //if the file end
                    if (file[i].Peek() < 0)
                    {
                        file.Remove(file[i]);
                        if (file.Count() == 0)
                        {
                            break;
                        }
                    }
                }

                //average
                if (counter != 0)
                {
                    double X = sum.X / (double)counter;
                    double Y = sum.Y / (double)counter;
                    //print avreage   
                    System.IO.File.AppendAllText(path + names[0] + "Average.txt", "0 0 0 0 0 " + jointNumber + " 0 0 " + X + " " + Y + " " + width + " " + height+ Environment.NewLine);
                }               

                //if all file ended
                if (file.Count() == 0)
                {
                    break;
                }
                
            }


            foreach (StreamReader fileName in file)
            {
                fileName.Close();
            }

        }
    }
}
