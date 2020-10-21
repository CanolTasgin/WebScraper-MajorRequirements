using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;


/* Workflow:
 * 1) One by one iterates through all majors 2010 to 2019 (+Fall and Spring Semesters) from Sabanci Website
 * 2) Make arrangements according to their specific pages so program can take the necessary information (There are differences between html pages)
 * 3) Takes first and last indexes of desired texts according to html pattern in the webpage
 * 4) Writes desired texts to the file by taking information between last and first indexes
 * 
 * There are at least 3 pages related with specific major. For example there is a main page for BSCS 2015 Fall Entry(Computer Science). And 3 other pages with Core, Area and Free Courses in them.
 * 
 * 5) Program one by one checks all webpages and outputs all information (Course code, Course Name and SU Credit) to the file. Main page is analized by all headers included in it.
*/

namespace FirstWebScraper
{
    class MainClass
    {
        public static void Main(string[] args) //Iterates trough all majors and calls another function that calls scraper
        {
            /////
            int faculty = 0;

            for (int i = 1; i <= 3; i++)
            {
                faculty = i;

                string majorcode = "";

                int delIndex; //I've used html patterns to reach specific texts. This integer is for deleting unnecessary extra occurrences.
                              //For example i've checked for '</a></td' in order to find lastIndexes of course names. There are different amount of '</a></td' in different webpages. I've arranged that with this delIndex

                if (faculty == 1)
                {
                    for (int j = 1; j <= 6; j++)
                    {
                        if (j == 1)
                        {
                            delIndex = 8;
                            majorcode = "BSCS";  //8
                            ScrapperCaller(delIndex, majorcode);
                        }

                        else if (j == 2)
                        {
                            delIndex = 8;
                            majorcode = "BSEE";  //8
                            ScrapperCaller(delIndex, majorcode);
                        }
                        else if (j == 3)
                        {
                            delIndex = 8;
                            majorcode = "BSMS";  //8
                            ScrapperCaller(delIndex, majorcode);
                        }
                        else if (j == 4)
                        {
                            majorcode = "BSMAT"; //6
                            delIndex = 6;
                            ScrapperCaller(delIndex, majorcode);
                        }
                        else if (j == 5)
                        {
                            delIndex = 8;
                            majorcode = "BSME";  //8
                            ScrapperCaller(delIndex, majorcode);
                        }
                        else if (j == 6)
                        {
                            majorcode = "BSBIO"; //6
                            delIndex = 6;
                            ScrapperCaller(delIndex, majorcode);
                        }
                    }
                }
                else if (faculty == 2)
                {
                    for (int k = 1; k <= 4; k++)
                    {
                        if (k == 1)
                        {
                            majorcode = "BAECON"; //7
                            delIndex = 7;
                            ScrapperCaller(delIndex, majorcode);
                        }
                        else if (k == 2)
                        {
                            majorcode = "BASPS";  //6
                            delIndex = 6;
                            ScrapperCaller(delIndex, majorcode);
                        }

                        else if (k == 3)
                        {
                            majorcode = "BAPSY";  //7
                            delIndex = 7;
                            ScrapperCaller(delIndex, majorcode);
                        }

                        else if (k == 4)
                        {
                            majorcode = "BAVACD"; //7}
                            delIndex = 7;
                            ScrapperCaller(delIndex, majorcode);
                        }
                    }
                }
                else if (faculty == 3)
                {
                    majorcode = "BAMAN";      //6
                    delIndex = 6;
                    ScrapperCaller(delIndex, majorcode); 
                }
            }
        }

        public static void ScrapperCaller(int delInd, string majorcod) // Iterates through entry years and semesters and calls scraper
        {
            string yearSemester = "";

            for (int i = 2010; i <= 2019; i++)
            {
                for (int j = 1; j <= 2; j++)
                {
                    yearSemester = i.ToString() + "0" + j.ToString();
                    string url1 = "https://www.sabanciuniv.edu/tr/aday-ogrenciler/degree-detail?SU_DEGREE.p_degree_detail%3fP_TERM=" + yearSemester + "&P_PROGRAM=" + majorcod + "&P_SUBMIT=&P_LANG=TR&P_LEVEL=UG";
                    if (i >= 2017 && majorcod == "BSMAT")
                        delInd = 8;
                    else if (i >= 2014 && majorcod == "BASPS")
                        delInd = 9;
                    Scraper(url1, delInd, yearSemester, majorcod);
                }
            }
        }

        public static void Scraper(string url, int a, string entr, string mjcode) 
        {
            //Create major directory if not already exists
            if (Directory.Exists(@mjcode))
            {
                Console.WriteLine(mjcode + "directory already exists. Moving on");
            }
            else
                Directory.CreateDirectory(@mjcode);

            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(@mjcode+"/"+mjcode+"_"+entr)) //Create a folder with current major code and create a file as 'majorCode_entryYear'
            {
                WebRequest request = WebRequest.Create(url); 

                WebResponse response = request.GetResponse();

                Console.WriteLine(((HttpWebResponse)response).StatusDescription); //Confirm the connection

                using (Stream dataStream = response.GetResponseStream())
                {
                    // Open the stream using a StreamReader for easy access.
                    StreamReader reader = new StreamReader(dataStream);
                    // Read the content.
                    string responseFromServer = reader.ReadToEnd(); // get the server response (html)

                    List<int> indexes = new List<int>();
                    for (int index = 0; ; index += "tur\" >".Length)
                    {
                        index = responseFromServer.IndexOf("tur\" >", index);
                        if (index == -1)
                            break;
                        indexes.Add(index);
                    }

                    List<int> lastIndexes = new List<int>();
                    for (int index = 0; ; index += "</a></td".Length)
                    {
                        index = responseFromServer.IndexOf("</a></td", index);
                        if (index == -1)
                            break;
                        lastIndexes.Add(index);
                    }

                    for (int i = 0; i < a; i++)
                        lastIndexes.RemoveAt(0);

                    while (lastIndexes.Count() != indexes.Count())
                        lastIndexes.RemoveAt(lastIndexes.Count - 1);

                    List<int> credits = new List<int>();
                    for (int index = 0; ; index += "width:60px;text-align:center;\">".Length)
                    {
                        index = responseFromServer.IndexOf("width:60px;text-align:center;\">", index);
                        if (index == -1)
                            break;
                        credits.Add(index);
                    }

                    List<int> headersFirst = new List<int>();
                    for (int index = 0; ; index += "</a><b>".Length)
                    {
                        index = responseFromServer.IndexOf("</a><b>", index);
                        if (index == -1)
                            break;
                        headersFirst.Add(index);
                    }

                    List<int> headersLast = new List<int>();
                    for (int index = 0; ; index += "</b></p>".Length)
                    {
                        index = responseFromServer.IndexOf("</b></p>", index);
                        if (index == -1)
                            break;
                        headersLast.Add(index);
                    }

                    List<string> headers = new List<string>();
                    for (int i = 0; i < headersFirst.Count(); i++)
                    {
                        headers.Add(responseFromServer.Substring(headersFirst[i] + "</a><b>".Length, headersLast[i] - headersFirst[i] - "</a><b>".Length));
                    }
                    int headerRemove = 0;
                    for (int j = 0; j < headersFirst.Count(); j++)
                    {
                        for (int i = 0; i < indexes.Count(); i++)
                        {
                            if (headersFirst[j] <= indexes[i])
                            {
                                indexes.Insert(i, -1);
                                lastIndexes.Insert(i, -1);
                                headerRemove++;
                                if (i % 2 != 0)
                                    credits.Insert(i / 2, -1);
                                break;
                            }
                        }
                    }

                    // Get necessary information from main page
                    int k = 0;
                    for (int i = 0; i < indexes.Count(); i++)
                    {
                        if (indexes[i] != -1)
                            file.WriteLine(responseFromServer.Substring(indexes[i] + "tur\" >".Length, lastIndexes[i] - indexes[i] - "tur\" >".Length));
                        else if (indexes[i] == -1)
                        {
                            file.WriteLine("\n" + headers[k] + "\n");
                            k++;
                        }

                        if (i % 2 != 0)
                            if (responseFromServer.Substring(credits[i / 2] + "width:60px;text-align:center;\">".Length, 1) != "")
                                file.WriteLine(responseFromServer.Substring(credits[i / 2] + "width:60px;text-align:center;\">".Length, 1));
                    }

                    for (int i = 0; i < headerRemove; i++)
                        headers.RemoveAt(0);

                    string areaLink = "_AEL";
                    string freeLink = "_FEL";

                    if (mjcode == "BAPSY" || mjcode == "BAMAN" || mjcode == "BSEE")
                    {
                        areaLink = "_ARE";
                        freeLink = "_FRE";
                    }

                    // Get necessary information from other pages
                    for (int ind = 0; ind < headers.Count(); ind++)
                    {
                        if (headers[ind] == "Çekirdek Seçmeli Dersler")
                        {
                            string urlCore = "https://www.sabanciuniv.edu/tr/aday-ogrenciler/degree-detail?SU_DEGREE_p_list_courses%3FP_TERM=" + entr + "&P_AREA=" + mjcode + "_CEL&P_PROGRAM=" + mjcode + "&P_LANG=TUR&P_LEVEL=UG";

                            WebRequest request1 = WebRequest.Create(urlCore);

                            WebResponse response1 = request1.GetResponse();

                            Console.WriteLine(((HttpWebResponse)response1).StatusDescription);

                            using (Stream dataStream1 = response1.GetResponseStream())
                            {
                                // Open the stream using a StreamReader for easy access.
                                StreamReader reader1 = new StreamReader(dataStream1);
                                // Read the content.
                                string responseCore = reader1.ReadToEnd();

                                List<int> coreIndexes = new List<int>();
                                for (int index = 0; ; index += "tur\" >".Length)
                                {
                                    index = responseCore.IndexOf("tur\" >", index);
                                    if (index == -1)
                                        break;
                                    coreIndexes.Add(index);
                                }

                                //counter lari sildim
                                List<int> coreLastIndexes = new List<int>();
                                for (int index = 0; ; index += "</a></td".Length)
                                {
                                    index = responseCore.IndexOf("</a></td", index);
                                    if (index == -1)
                                        break;
                                    coreLastIndexes.Add(index);
                                }

                                coreLastIndexes.RemoveAt(0);

                                List<int> coreCredits = new List<int>();
                                for (int index = 0; ; index += "width:60px;text-align:center;\">".Length)
                                {
                                    index = responseCore.IndexOf("width:60px;text-align:center;\">", index);
                                    if (index == -1)
                                        break;
                                    coreCredits.Add(index);
                                }

                                file.WriteLine("\n" + headers[ind] + "\n");

                                for (int i = 0; i < coreIndexes.Count(); i++)
                                {
                                    if (coreIndexes[i] != -1)
                                        file.WriteLine(responseCore.Substring(coreIndexes[i] + "tur\" >".Length, coreLastIndexes[i] - coreIndexes[i] - "tur\" >".Length));

                                    if (i % 2 != 0)
                                        if (responseCore.Substring(coreCredits[i / 2] + "width:60px;text-align:center;\">".Length, 1) != "")
                                            file.WriteLine(responseCore.Substring(coreCredits[i / 2] + "width:60px;text-align:center;\">".Length, 1));
                                }
                            }
                        }
                        else if (headers[ind] == "Alan Seçmeli Dersler" || headers[ind] == "Alan Dersleri")
                        {
                            string urlArea = "https://www.sabanciuniv.edu/tr/aday-ogrenciler/degree-detail?SU_DEGREE_p_list_courses%3FP_TERM=" + entr + "&P_AREA=" + mjcode + areaLink + "&P_PROGRAM=" + mjcode + "&P_LANG=TUR&P_LEVEL=UG";

                            WebRequest request2 = WebRequest.Create(urlArea);

                            WebResponse response2 = request2.GetResponse();

                            Console.WriteLine(((HttpWebResponse)response2).StatusDescription);

                            using (Stream dataStream2 = response2.GetResponseStream())
                            {
                                // Open the stream using a StreamReader for easy access.
                                StreamReader reader2 = new StreamReader(dataStream2);
                                // Read the content.
                                string responseArea = reader2.ReadToEnd();

                                List<int> areaIndexes = new List<int>();
                                for (int index = 0; ; index += "tur\" >".Length)
                                {
                                    index = responseArea.IndexOf("tur\" >", index);
                                    if (index == -1)
                                        break;
                                    areaIndexes.Add(index);
                                }

                                List<int> areaLastIndexes = new List<int>();
                                for (int index = 0; ; index += "</a></td".Length)
                                {
                                    index = responseArea.IndexOf("</a></td", index);
                                    if (index == -1)
                                        break;
                                    areaLastIndexes.Add(index);
                                }

                                areaLastIndexes.RemoveAt(0);

                                List<int> areaCredits = new List<int>();
                                for (int index = 0; ; index += "width:60px;text-align:center;\">".Length)
                                {
                                    index = responseArea.IndexOf("width:60px;text-align:center;\">", index);
                                    if (index == -1)
                                        break;
                                    areaCredits.Add(index);
                                }

                                file.WriteLine("\n" + headers[ind] + "\n");

                                for (int i = 0; i < areaIndexes.Count(); i++)
                                {
                                    if (areaIndexes[i] != -1)
                                        file.WriteLine(responseArea.Substring(areaIndexes[i] + "tur\" >".Length, areaLastIndexes[i] - areaIndexes[i] - "tur\" >".Length));

                                    if (i % 2 != 0)
                                        if (responseArea.Substring(areaCredits[i / 2] + "width:60px;text-align:center;\">".Length, 1) != "")
                                            file.WriteLine(responseArea.Substring(areaCredits[i / 2] + "width:60px;text-align:center;\">".Length, 1));
                                }
                            }
                        }
                        else if (headers[ind] == "Serbest Seçmeli Dersler")
                        {
                            string urlFree = "https://www.sabanciuniv.edu/tr/aday-ogrenciler/degree-detail?SU_DEGREE_p_list_courses%3FP_TERM=" + entr + "&P_AREA=" + mjcode + freeLink + "&P_PROGRAM=" + mjcode + "&P_LANG=TUR&P_LEVEL=UG";

                            WebRequest request3 = WebRequest.Create(urlFree);

                            WebResponse response3 = request3.GetResponse();

                            Console.WriteLine(((HttpWebResponse)response3).StatusDescription);

                            using (Stream dataStream3 = response3.GetResponseStream())
                            {
                                // Open the stream using a StreamReader for easy access.
                                StreamReader reader3 = new StreamReader(dataStream3);
                                // Read the content.
                                string responseFree = reader3.ReadToEnd();

                                List<int> freeIndexes = new List<int>();
                                for (int index = 0; ; index += "tur\" >".Length)
                                {
                                    index = responseFree.IndexOf("tur\" >", index);
                                    if (index == -1)
                                        break;
                                    freeIndexes.Add(index);
                                }

                                List<int> freeLastIndexes = new List<int>();
                                for (int index = 0; ; index += "</a></td".Length)
                                {
                                    index = responseFree.IndexOf("</a></td", index);
                                    if (index == -1)
                                        break;
                                    freeLastIndexes.Add(index);
                                }

                                freeLastIndexes.RemoveAt(0);

                                List<int> freeCredits = new List<int>();
                                for (int index = 0; ; index += "width:60px;text-align:center;\">".Length)
                                {
                                    index = responseFree.IndexOf("width:60px;text-align:center;\">", index);
                                    if (index == -1)
                                        break;
                                    freeCredits.Add(index);
                                }

                                file.WriteLine("\n" + headers[ind] + "\n");


                                for (int i = 0; i < freeIndexes.Count(); i++)
                                {
                                    if (freeIndexes[i] != -1)
                                        file.WriteLine(responseFree.Substring(freeIndexes[i] + "tur\" >".Length, freeLastIndexes[i] - freeIndexes[i] - "tur\" >".Length));

                                    if (i % 2 != 0)
                                        if (responseFree.Substring(freeCredits[i / 2] + "width:60px;text-align:center;\">".Length, 1) != "")
                                            file.WriteLine(responseFree.Substring(freeCredits[i / 2] + "width:60px;text-align:center;\">".Length, 1));
                                }

                            }
                        }
                    }
                }
            }
        }
    }
}