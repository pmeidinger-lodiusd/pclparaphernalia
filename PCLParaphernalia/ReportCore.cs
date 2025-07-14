using Microsoft.Win32;
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Xml;

namespace PCLParaphernalia
{
    /// <summary>
    /// 
    /// Class provides the core Report functions.
    /// 
    /// © Chris Hutchinson 2017
    /// 
    /// </summary>

    static class ReportCore
    {
        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Constants and enumerations.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        const int _lcSep = PrnParseConstants.cColSeparatorLen;

        public static string _chkMarkBoxSymFalse = '\u2610'.ToString();
        public static string _chkMarkBoxSymTrue = '\u2611'.ToString();

        public static string _chkMarkTxtSymFalse = '\x2d'.ToString();
        public static string _chkMarkTxtSymTrue = '\x2b'.ToString();

        public static string _chkMarkTextFalse = "false";
        public static string _chkMarkTextTrue = "true ";

        public enum eRptFileFmt : byte
        {
            text,
            html,
            xml,
            NA
        }

        public enum eRptChkMarks : byte
        {
            text,
            txtsym,
            boxsym,
            NA
        }

        //--------------------------------------------------------------------//
        //                                                        F i e l d s //
        // Fields (class variables).                                          //
        //                                                                    //
        //--------------------------------------------------------------------//

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d o c C l o s e                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Close report stream.                                               //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void DocClose(eRptFileFmt rptFileFmt,
                                     object stream,
                                     object writer)
        {
            if (rptFileFmt == eRptFileFmt.html)
            {
                HtmlTextWriter htmlWriter = (HtmlTextWriter)writer;
                htmlWriter.Close();
                writer = null;

                StreamWriter htmlStream = (StreamWriter)stream;

                htmlStream.Close();
                stream = null;
            }
            else if (rptFileFmt == eRptFileFmt.xml)
            {
                XmlWriter xmlWriter = (XmlWriter)writer;
                xmlWriter.Close();
                writer = null;

                StreamWriter xmlStream = (StreamWriter)stream;

                xmlStream.Close();
                stream = null;
            }
            else
            {
                StreamWriter txtWriter = (StreamWriter)writer;

                txtWriter.Close();
                writer = null;
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d o c F i n a l i s e                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write closing elements to report document.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void DocFinalise(eRptFileFmt rptFileFmt,
                                        object writer)
        {
            if (rptFileFmt == eRptFileFmt.html)
                DocFinaliseHtml((HtmlTextWriter)writer);
            else if (rptFileFmt == eRptFileFmt.xml)
                DocFinaliseXml((XmlWriter)writer);
            else
                DocFinaliseText((StreamWriter)writer);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d o c F i n a l i s e H t m l                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write closing elements to report document (html format).           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void DocFinaliseHtml(HtmlTextWriter htmlWriter)
        {
            htmlWriter.RenderBeginTag("p");
            htmlWriter.Write("*** End of Report ***");
            htmlWriter.RenderEndTag();    // </p>

            htmlWriter.RenderEndTag();    // </body>

            htmlWriter.RenderEndTag();    // </html>
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d o c F i n a l i s e T e x t                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write closing elements to report document (text format).           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void DocFinaliseText(StreamWriter txtWriter)
        {
            txtWriter.WriteLine(string.Empty);
            txtWriter.WriteLine("*** End of Report ***");
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d o c F i n a l i s e X m l                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write closing elements to report document (xml format).            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void DocFinaliseXml(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("trailer");

            xmlWriter.WriteStartElement("text");
            xmlWriter.WriteString("*** End of Report ***");
            xmlWriter.WriteEndElement();   // </text>

            xmlWriter.WriteEndElement();   // </trailer>

            xmlWriter.WriteEndDocument();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d o c I n i t i a l i s e                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write opening elements to report document.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void DocInitialise(eRptFileFmt rptFileFmt,
                                          object writer,
                                          bool useTables,
                                          bool useLines,
                                          int ctRowClrStyles,
                                          string[] rowClasses,
                                          string[] rowClrBack,
                                          string[] rowClrFore)
        {
            if (rptFileFmt == eRptFileFmt.html)
                DocInitialiseHtml((HtmlTextWriter)writer,
                                   ctRowClrStyles,
                                   rowClasses,
                                   rowClrBack,
                                   rowClrFore);
            else if (rptFileFmt == eRptFileFmt.xml)
                DocInitialiseXml((XmlWriter)writer,
                                   useTables,
                                   useLines,
                                   ctRowClrStyles,
                                   rowClasses,
                                   rowClrBack,
                                   rowClrFore);
            else
                DocInitialiseText((StreamWriter)writer);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d o c I n i t i a l i s e H t m l                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write opening elements to report document (html format).           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void DocInitialiseHtml(HtmlTextWriter htmlWriter,
                                              int ctRowClrStyles,
                                              string[] rowClasses,
                                              string[] rowClrBack,
                                              string[] rowClrFore)
        {
            //----------------------------------------------------------------//
            //                                                                //
            // Write out the html header tags.                                //
            //                                                                //
            //----------------------------------------------------------------//

            htmlWriter.WriteLine("<!doctype html>");

            htmlWriter.RenderBeginTag("html");

            htmlWriter.RenderBeginTag("head");

            htmlWriter.AddAttribute("charset", "utf-8");
            htmlWriter.RenderBeginTag("meta");
            htmlWriter.RenderEndTag();
            htmlWriter.WriteLine(string.Empty);

            //----------------------------------------------------------------//
            //                                                                //
            // Write the standard style definitions, and (if required) the    //
            // optional row colour coding style definitions.                  //
            //                                                                //
            //----------------------------------------------------------------//

            htmlWriter.RenderBeginTag("style");

            DocInitStyles(eRptFileFmt.html, htmlWriter, ctRowClrStyles,
                           rowClasses, rowClrBack, rowClrFore);

            //----------------------------------------------------------------//
            //                                                                //
            // Write out the html header end tags and start body element.     //
            //                                                                //
            //----------------------------------------------------------------//

            htmlWriter.RenderEndTag();    // </style>

            htmlWriter.RenderEndTag();    // </head>
            htmlWriter.WriteLine(string.Empty);

            htmlWriter.RenderBeginTag("body");
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d o c I n i t i a l i s e T e x t                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write opening elements to report document (text format).           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void DocInitialiseText(StreamWriter txtWriter)
        {
            // ******* nothing to do *******
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d o c I n i t i a l i s e X m l                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write opening elements to report document (xml format).            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void DocInitialiseXml(XmlWriter xmlWriter,
                                             bool useTables,
                                             bool useLines,
                                             int ctRowClrStyles,
                                             string[] rowClasses,
                                             string[] rowClrBack,
                                             string[] rowClrFore)
        {
            xmlWriter.WriteStartDocument();

            //----------------------------------------------------------------//
            //                                                                //
            // Write the Processing Instruction node.                         //
            //                                                                //
            //----------------------------------------------------------------//

            xmlWriter.WriteProcessingInstruction(
                "xml-stylesheet",
                "type=\"text/xsl\" href=\"#stylesheet\"");

            //----------------------------------------------------------------//
            //                                                                //
            // Write the DocumentType node.                                   //
            //                                                                //
            //----------------------------------------------------------------//

            xmlWriter.WriteDocType("report", null, null,
                                    "<!ATTLIST xsl:stylesheet id ID #REQUIRED>");

            //----------------------------------------------------------------//
            //                                                                //
            // Write the root element.                                        //
            //                                                                //
            //----------------------------------------------------------------//

            xmlWriter.WriteStartElement("report");

            xmlWriter.WriteComment("Start XSL");

            //----------------------------------------------------------------//
            //                                                                //
            // Start stylesheet definition.                                   //
            //                                                                //
            //----------------------------------------------------------------//

            xmlWriter.WriteStartElement("xsl", "stylesheet",
                                         "http://www.w3.org/1999/XSL/Transform");
            xmlWriter.WriteAttributeString("id", "stylesheet");
            xmlWriter.WriteAttributeString("version", "1.0");

            xmlWriter.WriteStartElement("xsl", "output", null);
            xmlWriter.WriteAttributeString("indent", "yes");
            xmlWriter.WriteAttributeString("method", "html");
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("xsl", "template", null);
            xmlWriter.WriteAttributeString("match", "xsl:stylesheet");
            xmlWriter.WriteEndElement();

            //----------------------------------------------------------------//
            //                                                                //
            // Template: styles                                               //
            //                                                                //
            //----------------------------------------------------------------//

            xmlWriter.WriteComment("template: styles");

            xmlWriter.WriteStartElement("xsl", "template", null);
            xmlWriter.WriteAttributeString("match", "/report");

            xmlWriter.WriteStartElement("html");
            xmlWriter.WriteStartElement("head");
            xmlWriter.WriteStartElement("style");
            //     xmlWriter.WriteStartElement ("xsl", "comment", null);

            DocInitStyles(eRptFileFmt.xml, xmlWriter, ctRowClrStyles,
                           rowClasses, rowClrBack, rowClrFore);

            //     xmlWriter.WriteEndElement ();               // </xsl:comment>
            xmlWriter.WriteEndElement();               // </style
            xmlWriter.WriteEndElement();               // </head>

            xmlWriter.WriteStartElement("body");
            xmlWriter.WriteStartElement("xsl", "apply-templates", null);
            xmlWriter.WriteEndElement();               // 

            xmlWriter.WriteEndElement();               // </body
            xmlWriter.WriteEndElement();               // </html>
            xmlWriter.WriteEndElement();               // </xsl:template>

            //----------------------------------------------------------------//
            //                                                                //
            // Template: report header                                        //
            //                                                                //
            //----------------------------------------------------------------//

            xmlWriter.WriteComment("template: header");

            xmlWriter.WriteStartElement("xsl", "template", null);
            xmlWriter.WriteAttributeString("match", "/report/header");

            xmlWriter.WriteStartElement("p");
            xmlWriter.WriteAttributeString("class", "title");

            xmlWriter.WriteStartElement("xsl", "value-of", null);
            xmlWriter.WriteAttributeString("select", "title");
            xmlWriter.WriteEndElement();               // </xsl:value-of>
            xmlWriter.WriteEndElement();               // </p>

            xmlWriter.WriteEndElement();               // </xsl:template>

            if (useLines)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Template: report lineblock                                 //
                //                                                            //
                //------------------------------------------------------------//

                xmlWriter.WriteStartElement("xsl", "template", null);
                xmlWriter.WriteAttributeString("match", "/report/lineblock");

                xmlWriter.WriteStartElement("xsl", "for-each", null);
                xmlWriter.WriteAttributeString("select", "item/value");

                xmlWriter.WriteStartElement("p");

                xmlWriter.WriteStartElement("xsl", "value-of", null);
                xmlWriter.WriteAttributeString("select", ".");

                xmlWriter.WriteEndElement();       // </xsl:value-of>
                xmlWriter.WriteEndElement();       // </p>
                xmlWriter.WriteEndElement();       // </xsl:for-each>

                xmlWriter.WriteEndElement();               // </xsl:template>
            }

            if (useTables)
            {
                //------------------------------------------------------------//
                //                                                            //
                // Template: report tabledata                                 //
                //                                                            //
                //------------------------------------------------------------//

                xmlWriter.WriteComment("template: tabledata");

                xmlWriter.WriteStartElement("xsl", "template", null);
                xmlWriter.WriteAttributeString("match", "/report/tabledata");

                xmlWriter.WriteStartElement("table");

                //------------------------------------------------------------//

                xmlWriter.WriteComment("template: tabledata hddr");

                xmlWriter.WriteStartElement("tr");

                xmlWriter.WriteStartElement("xsl", "for-each", null);
                xmlWriter.WriteAttributeString("select", "hddr/col");
                xmlWriter.WriteStartElement("th");
                xmlWriter.WriteStartElement("xsl", "attribute", null);
                xmlWriter.WriteAttributeString("name", "class");
                xmlWriter.WriteStartElement("xsl", "value-of", null);
                xmlWriter.WriteAttributeString("select", "../@hddrstyle");
                xmlWriter.WriteEndElement();       // </xsl:value-of>
                xmlWriter.WriteEndElement();       // </xsl:attribute>

                xmlWriter.WriteStartElement("xsl", "value-of", null);
                xmlWriter.WriteAttributeString("select", ".");

                xmlWriter.WriteEndElement();       // </xsl:value-of>
                xmlWriter.WriteEndElement();       // </th>
                xmlWriter.WriteEndElement();       // </xsl:for-each>

                xmlWriter.WriteEndElement();       // </tr>

                //------------------------------------------------------------//

                xmlWriter.WriteComment("template: tabledata item");

                xmlWriter.WriteStartElement("xsl", "for-each", null);
                xmlWriter.WriteAttributeString("select", "item");

                xmlWriter.WriteStartElement("tr");
                xmlWriter.WriteStartElement("xsl", "attribute", null);
                xmlWriter.WriteAttributeString("name", "class");
                xmlWriter.WriteStartElement("xsl", "value-of", null);
                xmlWriter.WriteAttributeString("select", "@rowType");
                xmlWriter.WriteEndElement();       // </xsl:value-of>
                xmlWriter.WriteEndElement();       // </xsl:attribute>

                xmlWriter.WriteStartElement("xsl", "for-each", null);
                xmlWriter.WriteAttributeString("select", "*");
                xmlWriter.WriteStartElement("td");

                xmlWriter.WriteStartElement("xsl", "attribute", null);
                xmlWriter.WriteAttributeString("name", "class");
                xmlWriter.WriteStartElement("xsl", "value-of", null);
                xmlWriter.WriteAttributeString(
                    "select", "concat(../@padType, ' ', @txtfmt)");
                xmlWriter.WriteEndElement();       // </xsl:value-of>
                xmlWriter.WriteEndElement();       // </xsl:attribute>

                xmlWriter.WriteStartElement("xsl", "attribute", null);
                xmlWriter.WriteAttributeString("name", "colspan");
                xmlWriter.WriteStartElement("xsl", "value-of", null);
                xmlWriter.WriteAttributeString("select", "@colspan");
                xmlWriter.WriteEndElement();       // </xsl:value-of>
                xmlWriter.WriteEndElement();       // </xsl:attribute>

                xmlWriter.WriteStartElement("xsl", "value-of", null);
                xmlWriter.WriteAttributeString("select", ".");

                xmlWriter.WriteEndElement();       // </xsl:value-of>
                xmlWriter.WriteEndElement();       // </td>
                xmlWriter.WriteEndElement();       // </xsl:for-each>

                xmlWriter.WriteEndElement();       // </tr>
                xmlWriter.WriteEndElement();       // </xsl:for-each>

                //------------------------------------------------------------//

                xmlWriter.WriteEndElement();       // </table>

                xmlWriter.WriteEndElement();       // </xsl:template>

                //------------------------------------------------------------//
                //                                                            //
                // Template: report tablepair                                 //
                //                                                            //
                //------------------------------------------------------------//

                xmlWriter.WriteComment("template: tablepair");

                xmlWriter.WriteStartElement("xsl", "template", null);
                xmlWriter.WriteAttributeString("match", "/report/tablepair");

                xmlWriter.WriteStartElement("table");

                //------------------------------------------------------------//

                xmlWriter.WriteStartElement("xsl", "for-each", null);
                xmlWriter.WriteAttributeString("select", "item");

                xmlWriter.WriteStartElement("tr");

                xmlWriter.WriteStartElement("xsl", "for-each", null);
                xmlWriter.WriteAttributeString("select", "*");
                xmlWriter.WriteStartElement("td");

                xmlWriter.WriteStartElement("xsl", "attribute", null);
                xmlWriter.WriteAttributeString("name", "class");
                xmlWriter.WriteStartElement("xsl", "value-of", null);
                xmlWriter.WriteAttributeString("select", "../@padType");
                xmlWriter.WriteEndElement();       // </xsl:value-of>
                xmlWriter.WriteEndElement();       // </xsl:attribute>

                xmlWriter.WriteStartElement("xsl", "value-of", null);
                xmlWriter.WriteAttributeString("select", ".");

                xmlWriter.WriteEndElement();       // </xsl:value-of>
                xmlWriter.WriteEndElement();       // </td>
                xmlWriter.WriteEndElement();       // </xsl:for-each>

                xmlWriter.WriteEndElement();       // </tr>
                xmlWriter.WriteEndElement();       // </xsl:for-each>

                //------------------------------------------------------------//

                xmlWriter.WriteEndElement();       // </table>

                xmlWriter.WriteEndElement();       // </xsl:template>
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Template: report trailer                                       //
            //                                                                //
            //----------------------------------------------------------------//

            xmlWriter.WriteComment("template: trailer");

            xmlWriter.WriteStartElement("xsl", "template", null);
            xmlWriter.WriteAttributeString("match", "/report/trailer");

            xmlWriter.WriteStartElement("p");

            xmlWriter.WriteStartElement("xsl", "value-of", null);
            xmlWriter.WriteAttributeString("select", "text");
            xmlWriter.WriteEndElement();               // </xsl:value-of>
            xmlWriter.WriteEndElement();               // </p>
            xmlWriter.WriteEndElement();               // </xsl:template>

            //----------------------------------------------------------------//
            //                                                                //
            // end of stylesheet definition                                   //
            //                                                                //
            //----------------------------------------------------------------//

            xmlWriter.WriteEndElement();               // </xsl:stylesheet>

            //----------------------------------------------------------------//
            //                                                                //
            // Start XML                                                      //
            //                                                                //
            //----------------------------------------------------------------//

            xmlWriter.WriteComment("Start XML");
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d o c I n i t S t y l e s                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write standard style definitions and (if required) the optional    //
        // row colour coding style definitions to report document.            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void DocInitStyles(eRptFileFmt rptFileFmt,
                                          object writer,
                                          int ctRowClrStyles,
                                          string[] rowClasses,
                                          string[] rowClrBack,
                                          string[] rowClrFore)
        {
            string[] stdStyles =
            {
                "p",
                "{",
                "\t" + "font-family: Courier; ",
                "\t" + "font - size: 100 %; ",
                "\t" + "margin-top: 0em;",
                "\t" + "margin-bottom: 0em;",
                "\t" + "white-space: pre;",
                "}",

                "p.title",
                "{",
                "\t" + "font-weight: bold;",
                "}",

                "table",
                "{",
                "\t" + "font-family: Courier;",
                "\t" + "font-size: 100 %;",
                "\t" + "margin-left: 0em;",
                "\t" + "margin-top: 1em;",
                "\t" + "margin-bottom: 1em;",
                "\t" + "white-space: pre;",
                "}",

                "th",
                "{",
                "\t" + "font-style: italic;",
                "\t" + "font-weight: normal;",
                "\t" + "text-align: left;",
                "\t" + "text-decoration: underline;",
                "\t" + "padding-top: 0em;",
                "\t" + "padding-bottom: 0em;",
                "\t" + "vertical-align: top;",
                "}",

                "th.plain",
                "{",
                "\t" + "font-style: normal;",
                "\t" + "font-weight: normal;",
                "\t" + "text-align: left;",
                "\t" + "text-decoration: none;",
                "\t" + "padding-top: 0em;",
                "\t" + "padding-bottom: 0em;",
                "\t" + "vertical-align: top;",
                "}",

                "td",
                "{",
                "\t" + "padding-right: 1em;",
                "\t" + "padding-top: 0em;",
                "\t" + "padding-bottom: 0em;",
                "\t" + "vertical-align: top;",
                "}",

                ".fmtAdorn",
                "{",
                "\t" + "font-style: italic;",
                "\t" + "font-weight: normal;",
                "\t" + "text-align: left;",
                "\t" + "text-decoration: underline;",
                "}",

                ".fmtPlain",
                "{",
                "\t" + "font-style: normal;",
                "\t" + "font-weight: normal;",
                "\t" + "text-align: left;",
                "\t" + "text-decoration: none;",
                "}",

                ".padAnte",
                "{",
                "\t" + "padding-top: 1em;",
                "\t" + "padding-bottom: 0em;",
                "\t" + "vertical-align: top;",
                "}",

                ".padAntePost",
                "{",
                "\t" + "padding-top: 1em;",
                "\t" + "padding-bottom: 1em;",
                "\t" + "vertical-align: top;",
                "}",

                ".padPost",
                "{",
                "\t" + "padding-top: 0em;",
                "\t" + "padding-bottom: 1em;",
                "\t" + "vertical-align: top;",
                "}"
            };

            if (rptFileFmt == eRptFileFmt.html)
                DocInitStylesHtml(
                    (HtmlTextWriter)writer, stdStyles, ctRowClrStyles,
                    rowClasses, rowClrBack, rowClrFore);
            else if (rptFileFmt == eRptFileFmt.xml)
                DocInitStylesXml(
                    (XmlWriter)writer, stdStyles, ctRowClrStyles,
                    rowClasses, rowClrBack, rowClrFore);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d o c I n i t S t y l e s H t m l                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write standard style definitions and (if required) the optional    //
        // row colour coding style definitions to report document (html       //
        // format).                                                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void DocInitStylesHtml(HtmlTextWriter htmlWriter,
                                              string[] stdStyles,
                                              int ctRowClrStyles,
                                              string[] rowClasses,
                                              string[] rowClrBack,
                                              string[] rowClrFore)
        {
            //----------------------------------------------------------------//
            //                                                                //
            // Standard style definitions.                                    //
            //                                                                //
            //----------------------------------------------------------------//

            int ctLines = stdStyles.Length;

            for (int i = 0; i < ctLines; i++)
            {
                htmlWriter.WriteLine(stdStyles[i]);
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Optional row colour coding style definitions.                  //
            //                                                                //
            //----------------------------------------------------------------//

            if (ctRowClrStyles > 0)
            {
                htmlWriter.WriteLine(string.Empty);

                for (int i = 0; i < ctRowClrStyles; i++)
                {
                    htmlWriter.Write("tr." +
                                      rowClasses[i] + " {");
                    htmlWriter.Write(" background-color: " +
                                      rowClrBack[i] + ";");
                    htmlWriter.Write(" color: " +
                                      rowClrFore[i] + ";");

                    if (i == ctRowClrStyles)
                        htmlWriter.Write(" }");
                    else
                        htmlWriter.WriteLine(" }");
                }
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d o c I n i t S t y l e s X m l                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write standard style definitions and (if required) the optional    //
        // row colour coding style definitions to report document (xml        //
        // format).                                                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void DocInitStylesXml(XmlWriter xmlWriter,
                                             string[] stdStyles,
                                             int ctRowClrStyles,
                                             string[] rowClasses,
                                             string[] rowClrBack,
                                             string[] rowClrFore)
        {
            //----------------------------------------------------------------//
            //                                                                //
            // Standard style definitions.                                    //
            //                                                                //
            //----------------------------------------------------------------//

            int ctLines = stdStyles.Length;

            xmlWriter.WriteString("\r\n");

            for (int i = 0; i < ctLines; i++)
            {
                xmlWriter.WriteString("\t\t\t" + stdStyles[i] + "\r\n");
            }

            //----------------------------------------------------------------//
            //                                                                //
            // Optional row colour coding style definitions.                  //
            //                                                                //
            //----------------------------------------------------------------//

            if (ctRowClrStyles > 0)
            {
                for (int i = 0; i < ctRowClrStyles; i++)
                {
                    xmlWriter.WriteString("\t\t\t" + "tr." +
                                           rowClasses[i] + " {");
                    xmlWriter.WriteString(" background-color: " +
                                           rowClrBack[i] + ";");
                    xmlWriter.WriteString(" color: " +
                                           rowClrFore[i] + ";");
                    xmlWriter.WriteString(" }\r\n");
                }
            }

            xmlWriter.WriteString("\t\t");
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // d o c O p e n                                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Open report stream.                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static bool DocOpen(eRptFileFmt rptFileFmt,
                                       ref string saveFilename,
                                       ref object stream,
                                       ref object writer)
        {
            SaveFileDialog saveDialog = ToolCommonFunctions.CreateSaveFileDialog(saveFilename);

            if (rptFileFmt == eRptFileFmt.html)
            {
                saveDialog.Filter = "Html Files | *.html";
                saveDialog.DefaultExt = "html";
            }
            else if (rptFileFmt == eRptFileFmt.xml)
            {
                saveDialog.Filter = "Xml Files | *.xml";
                saveDialog.DefaultExt = "xml";
            }
            else
            {
                saveDialog.Filter = "Text Files | *.txt";
                saveDialog.DefaultExt = "txt";
            }

            bool? dialogResult = saveDialog.ShowDialog();
            bool fileOpen = false;

            if (dialogResult == true)
            {
                saveFilename = saveDialog.FileName;

                if (rptFileFmt == eRptFileFmt.html)
                {
                    stream = new StreamWriter(saveFilename);

                    if (stream != null)
                    {
                        writer = new HtmlTextWriter((StreamWriter)stream);
                        fileOpen = true;
                    }
                }
                else if (rptFileFmt == eRptFileFmt.xml)
                {
                    stream = new StreamWriter(saveFilename);

                    if (stream != null)
                    {
                        XmlWriterSettings settings = new XmlWriterSettings();
                        settings.Encoding = Encoding.UTF8;
                        settings.Indent = true;

                        writer = XmlWriter.Create((StreamWriter)stream, settings);
                        fileOpen = true;
                    }
                }
                else
                {
                    stream = null;
                    writer = new StreamWriter(saveFilename);

                    if (writer != null)
                    {
                        fileOpen = true;
                    }
                }
            }

            return fileOpen;
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // h d d r C l o s e                                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Close header section.                                              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void HddrClose(object writer,
                                      eRptFileFmt rptFileFmt)
        {
            if (rptFileFmt == eRptFileFmt.html)
                HddrCloseHtml((HtmlTextWriter)writer);
            else if (rptFileFmt == eRptFileFmt.xml)
                HddrCloseXml((XmlWriter)writer);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // h d d r C l o s e H t m l                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write close tag in html format.                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void HddrCloseHtml(HtmlTextWriter htmlWriter)
        {
            htmlWriter.RenderEndTag();     // </table>
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // h d d r C l o s e X m l                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write close tag in xml format.                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void HddrCloseXml(XmlWriter xmlWriter)
        {
            xmlWriter.WriteEndElement();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // h d d r T i t l e                                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write report header title line.                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void HddrTitle(object writer,
                                      eRptFileFmt rptFileFmt,
                                      bool subHddr,
                                      string txtVal)
        {
            if (rptFileFmt == eRptFileFmt.html)
                HddrTitleHtml((HtmlTextWriter)writer, txtVal);
            else if (rptFileFmt == eRptFileFmt.xml)
                HddrTitleXml((XmlWriter)writer, txtVal);
            else
                HddrTitleText((StreamWriter)writer, subHddr, txtVal);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // h d d r T i t l e H t m l                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write report header title line in html format.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void HddrTitleHtml(HtmlTextWriter htmlWriter,
                                           string txtVal)
        {
            htmlWriter.AddAttribute("class", "title");
            htmlWriter.RenderBeginTag("p");
            htmlWriter.WriteEncodedText(txtVal);
            htmlWriter.RenderEndTag();
            htmlWriter.WriteLine(string.Empty);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // h d d r T i t l e T e x t                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write report header title line in text format.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void HddrTitleText(StreamWriter txtWriter,
                                           bool subHddr,
                                           string txtVal)
        {
            if (subHddr)
                txtWriter.WriteLine(string.Empty);

            txtWriter.WriteLine(txtVal);

            if (subHddr)
            {
                int len = txtVal.Length;

                txtWriter.WriteLine("-".PadRight(len, '-'));
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // h d d r T i t l e X m l                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write report header title line in xml format.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void HddrTitleXml(XmlWriter xmlWriter,
                                          string txtVal)
        {
            xmlWriter.WriteStartElement("header");

            xmlWriter.WriteStartElement("title");

            xmlWriter.WriteString(txtVal);

            xmlWriter.WriteEndElement();           // </title>

            xmlWriter.WriteEndElement();           // </header>
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l i n e B l o c k C l o s e                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write line block close tag.                                        //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LineBlockClose(object writer,
                                          eRptFileFmt rptFileFmt)
        {
            if (rptFileFmt == eRptFileFmt.html)
                LineBlockCloseHtml((HtmlTextWriter)writer);
            else if (rptFileFmt == eRptFileFmt.xml)
                LineBlockCloseXml((XmlWriter)writer);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l i n e B l o c k C l o s e H t m l                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write close tag in html format.                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void LineBlockCloseHtml(HtmlTextWriter htmlWriter)
        {
            htmlWriter.RenderEndTag();
            htmlWriter.WriteLine(string.Empty);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l i n e B l o c k C l o s e X m l                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write close tag in xml format.                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void LineBlockCloseXml(XmlWriter xmlWriter)
        {
            xmlWriter.WriteEndElement();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l i n e B l o c k O p e n                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write line block open tag.                                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LineBlockOpen(object writer,
                                          eRptFileFmt rptFileFmt)
        {
            string tag = "lineblock";

            if (rptFileFmt == eRptFileFmt.html)
                LineBlockOpenHtml((HtmlTextWriter)writer, tag);
            else if (rptFileFmt == eRptFileFmt.xml)
                LineBlockOpenXml((XmlWriter)writer, tag);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l i n e B l o c k O p e n H t m l                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write line block  open tag in html format.                         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void LineBlockOpenHtml(HtmlTextWriter htmlWriter,
                                               string tag)
        {
            htmlWriter.RenderBeginTag(tag);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l i n e B l o c k O p e n X m l                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write line block open tag in xml format.                           //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void LineBlockOpenXml(XmlWriter xmlWriter,
                                              string txtName)
        {
            xmlWriter.WriteStartElement(txtName);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l i n e I t e m                                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write item containing just a value component.                      //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void LineItem(object writer,
                                     eRptFileFmt rptFileFmt,
                                     string txtVal,
                                     int sizeVal,
                                     bool firstItem)
        {
            if (rptFileFmt == eRptFileFmt.html)
                LineItemHtml((HtmlTextWriter)writer,
                              txtVal, sizeVal, firstItem);
            else if (rptFileFmt == eRptFileFmt.xml)
                LineItemXml((XmlWriter)writer, txtVal);
            else
                LineItemText((StreamWriter)writer, txtVal,
                              sizeVal, firstItem);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l i n e I t e m H t m l                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write line item in html format.                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void LineItemHtml(HtmlTextWriter htmlWriter,
                                          string txtVal,
                                          int maxSizeVal,
                                          bool firstItem)
        {
            int valPos,
                  valLen;

            valLen = txtVal.Length;
            valPos = 0;

            if (firstItem)
            {
                htmlWriter.RenderBeginTag("p");
                htmlWriter.Write("&nbsp;");
                htmlWriter.RenderEndTag();
                htmlWriter.WriteLine(string.Empty);
            }

            while (valPos + maxSizeVal < valLen)
            {
                htmlWriter.RenderBeginTag("p");
                htmlWriter.WriteEncodedText(
                        txtVal.Substring(valPos, maxSizeVal));
                htmlWriter.RenderEndTag();
                htmlWriter.WriteLine(string.Empty);

                valPos += maxSizeVal;
            }

            if (valPos <= valLen)
            {
                htmlWriter.RenderBeginTag("p");
                htmlWriter.WriteEncodedText(
                        txtVal.Substring(valPos, valLen - valPos));
                htmlWriter.RenderEndTag();
                htmlWriter.WriteLine(string.Empty);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l i n e I t e m T e x t                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write line item in text format.                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void LineItemText(StreamWriter txtWriter,
                                          string txtVal,
                                          int maxSizeVal,
                                          bool blankBefore)
        {
            int valPos,
                  valLen;

            if (blankBefore)
                txtWriter.WriteLine();

            valLen = txtVal.Length;
            valPos = 0;

            while (valPos + maxSizeVal < valLen)
            {
                txtWriter.WriteLine(
                        txtVal.Substring(valPos, maxSizeVal));

                valPos += maxSizeVal;
            }

            if (valPos <= valLen)
            {
                txtWriter.WriteLine(
                        txtVal.Substring(valPos, valLen - valPos));
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // l i n e I t e m X m l                                              //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write line item in xml format.                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void LineItemXml(XmlWriter xmlWriter,
                                         string txtVal)
        {
            xmlWriter.WriteStartElement("item");

            try
            {
                string validXml = XmlConvert.VerifyXmlChars(txtVal);

                xmlWriter.WriteStartElement("value");

                xmlWriter.WriteString(validXml);
            }
            catch
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(txtVal);

                string base64 = Convert.ToBase64String(bytes);

                xmlWriter.WriteStartElement("valuebase64");

                xmlWriter.WriteString(base64);
            }

            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();   // </item>
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a b l e C l o s e                                                //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Close report table.                                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void TableClose(object writer,
                                       eRptFileFmt rptFileFmt)
        {
            if (rptFileFmt == eRptFileFmt.html)
                TableCloseHtml((HtmlTextWriter)writer);
            else if (rptFileFmt == eRptFileFmt.xml)
                TableCloseXml((XmlWriter)writer);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a b l e C l o s e H t m l                                        //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write close tag in html format.                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void TableCloseHtml(HtmlTextWriter htmlWriter)
        {
            htmlWriter.RenderEndTag();
            htmlWriter.WriteLine(string.Empty);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a b l e C l o s e X m l                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write close tag in xml format.                                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void TableCloseXml(XmlWriter xmlWriter)
        {
            xmlWriter.WriteEndElement();
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a b l e H d d r D a t a                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Open report data table and write header row.                       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void TableHddrData(object writer,
                                          eRptFileFmt rptFileFmt,
                                          bool plain,
                                          int colCt,
                                          string[] colHddrs,
                                          int[] colSizes)
        {
            if (rptFileFmt == eRptFileFmt.html)
                TableHddrDataHtml((HtmlTextWriter)writer, plain,
                                   colCt, colHddrs);
            else if (rptFileFmt == eRptFileFmt.xml)
                TableHddrDataXml((XmlWriter)writer, plain, colCt, colHddrs);
            else
                TableHddrDataText((StreamWriter)writer, colCt, colHddrs,
                                   colSizes);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a b l e H d d r D a t a H t m l                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Open report data table and write header row in html format.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void TableHddrDataHtml(HtmlTextWriter htmlWriter,
                                               bool plain,
                                               int colCt,
                                               string[] colHddrs)
        {
            int lastCol = colCt - 1;

            htmlWriter.WriteLine(string.Empty);
            htmlWriter.RenderBeginTag("table");

            if (colCt > 0)
            {
                htmlWriter.RenderBeginTag("tr");

                for (int i = 0; i < colCt; i++)
                {
                    if (plain)
                        htmlWriter.AddAttribute("class", "plain");

                    htmlWriter.RenderBeginTag("th");

                    htmlWriter.WriteEncodedText(colHddrs[i].ToString());
                    htmlWriter.RenderEndTag();    // </td>

                    if (i != lastCol)
                        htmlWriter.WriteLine(string.Empty);
                }

                htmlWriter.RenderEndTag();    // </tr>
                htmlWriter.WriteLine(string.Empty);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a b l e H d d r D a t a T e x t                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Open report data table and write header row in text format.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void TableHddrDataText(StreamWriter txtWriter,
                                               int colCt,
                                               string[] colHddrs,
                                               int[] colSizes)
        {
            if (colCt > 0)
            {
                int lastCol = colCt - 1;

                string colSep = " ".PadRight(_lcSep, ' ');

                StringBuilder line = new StringBuilder();

                txtWriter.WriteLine(string.Empty);

                for (int i = 0; i < colCt; i++)
                {
                    line.Append(colHddrs[i].ToString().PadRight(colSizes[i],
                                                                   ' '));

                    if (i != lastCol)
                        line.Append(colSep);
                }

                txtWriter.WriteLine(line);

                line.Clear();

                for (int i = 0; i < colCt; i++)
                {
                    line.Append("-".PadRight(colSizes[i], '-'));

                    if (i != lastCol)
                        line.Append(colSep);
                }

                txtWriter.WriteLine(line);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a b l e H d d r D a t a X m l                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Open report data table and write header row in xml format.         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void TableHddrDataXml(XmlWriter xmlWriter,
                                              bool plain,
                                              int colCt,
                                              string[] colHddrs)
        {
            xmlWriter.WriteStartElement("tabledata");

            if (colCt > 0)
            {
                if (plain)
                {
                    xmlWriter.WriteStartElement("hddr");
                    xmlWriter.WriteAttributeString("hddrstyle", "plain");
                }
                else
                {
                    xmlWriter.WriteStartElement("hddr");
                }

                for (int i = 0; i < colCt; i++)
                {
                    xmlWriter.WriteStartElement("col");
                    xmlWriter.WriteString(colHddrs[i]);
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a b l e H d d r P a i r                                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Open report pair table.                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void TableHddrPair(object writer,
                                          eRptFileFmt rptFileFmt)
        {
            if (rptFileFmt == eRptFileFmt.html)
                TableHddrPairHtml((HtmlTextWriter)writer);
            else if (rptFileFmt == eRptFileFmt.xml)
                TableHddrPairXml((XmlWriter)writer);
            else
                TableHddrPairText((StreamWriter)writer);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a b l e H d d r P a i r H t m l                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Open report pair table and write header row in html format.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void TableHddrPairHtml(HtmlTextWriter htmlWriter)
        {
            htmlWriter.WriteLine(string.Empty);
            htmlWriter.RenderBeginTag("table");
            htmlWriter.WriteLine(string.Empty);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a b l e H d d r P a i r T e x t                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Open report data table and write header row in text format.        //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void TableHddrPairText(StreamWriter txtWriter)
        {
            txtWriter.WriteLine(string.Empty);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a b l e H d d r P a i r X m l                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Open report pair table and write header row in xml format.         //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void TableHddrPairXml(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("tablepair");
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a b l e M u l t i R o w T e x t                                  //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write report table row from data supplied as an array of string    //
        // arrays.                                                            //
        // Only required if output format is text?                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void TableMultiRowText(
            object writer,
            eRptFileFmt rptFileFmt,
            int colCt,
            string[][] arrData,
            int[] colSizes,
            bool blankBefore,
            bool blankAfter,
            bool blankAfterMultiRow)
        {
            if (rptFileFmt == eRptFileFmt.text)
            {
                TableMultiRowTextText(
                    (StreamWriter)writer, colCt, arrData, colSizes,
                    blankBefore, blankAfter, blankAfterMultiRow);

            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a b l e M u l t i R o w T e x t T e x t                          //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write report table row in text format.                             //
        // Data is supplied as an array of string arrays.                     //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void TableMultiRowTextText(
            StreamWriter txtWriter,
            int colCt,
            string[][] arrData,
            int[] colSizes,
            bool blankBefore,
            bool blankAfter,
            bool blankAfterMultiRow)
        {
            int lastCol = colCt - 1;

            int maxRows = 0;

            string space = " ";

            string colSep = " ".PadRight(_lcSep, ' ');

            StringBuilder line = new StringBuilder();

            for (int i = 0; i < colCt; i++)
            {
                int x = arrData[i].Length;

                if (x > maxRows)
                    maxRows = x;
            }

            if (blankBefore)
            {
                txtWriter.WriteLine(string.Empty);
            }

            for (int i = 0; i < maxRows; i++)
            {
                for (int j = 0; j < colCt; j++)
                {
                    if (arrData[j].Length <= i)
                        line.Append(space.PadRight(colSizes[j], ' '));
                    else
                        line.Append((arrData[j][i]).PadRight(colSizes[j], ' '));

                    if (j != lastCol)
                        line.Append(colSep);
                }

                txtWriter.WriteLine(line);
                line.Clear();
            }

            if ((blankAfter) || ((blankAfterMultiRow) && (maxRows > 1)))
            {
                txtWriter.WriteLine(string.Empty);
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a b l e R o w D a t a                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write report table row from data supplied as a data row.           //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void TableRowData(object writer,
                                         eRptFileFmt rptFileFmt,
                                         eRptChkMarks rptChkMarks,
                                         int colCt,
                                         string rowType,
                                         DataRow row,
                                         string[] colNames,
                                         int[] colSizes)
        {
            if (rptFileFmt == eRptFileFmt.html)
                TableRowDataHtml((HtmlTextWriter)writer, rptChkMarks,
                                  colCt, rowType, row, colNames);
            else if (rptFileFmt == eRptFileFmt.xml)
                TableRowDataXml((XmlWriter)writer, rptChkMarks,
                                 colCt, rowType, row, colNames);
            else
                TableRowDataText((StreamWriter)writer, rptChkMarks,
                                  colCt, rowType, row, colNames, colSizes);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a b l e R o w D a t a H t m l                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write report table row in html format.                             //
        // Data is supplied as a data row.                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void TableRowDataHtml(HtmlTextWriter htmlWriter,
                                              eRptChkMarks rptChkMarks,
                                              int colCt,
                                              string rowType,
                                              DataRow row,
                                              string[] colNames)
        {
            int lastCol = colCt - 1;

            if (rowType != null)
                htmlWriter.AddAttribute("class", rowType);

            htmlWriter.RenderBeginTag("tr");

            for (int i = 0; i < colCt; i++)
            {
                htmlWriter.RenderBeginTag("td");

                if (row[colNames[i]] is bool)
                {
                    if ((bool)row[colNames[i]] == true)
                    {
                        if (rptChkMarks == eRptChkMarks.boxsym)
                            htmlWriter.Write(_chkMarkBoxSymTrue);
                        else if (rptChkMarks == eRptChkMarks.txtsym)
                            htmlWriter.Write(_chkMarkTxtSymTrue);
                        else
                            htmlWriter.Write(_chkMarkTextTrue);
                    }
                    else
                    {
                        if (rptChkMarks == eRptChkMarks.boxsym)
                            htmlWriter.Write(_chkMarkBoxSymFalse);
                        else if (rptChkMarks == eRptChkMarks.txtsym)
                            htmlWriter.Write(_chkMarkTxtSymFalse);
                        else
                            htmlWriter.Write(_chkMarkTextFalse);
                    }
                }
                else if ((i == 0) && (row[colNames[0]].ToString() == string.Empty))
                {
                    htmlWriter.Write("&nbsp;");
                }
                else
                {
                    htmlWriter.WriteEncodedText(row[colNames[i]].ToString());
                }

                htmlWriter.RenderEndTag();    // </td>

                if (i != lastCol)
                    htmlWriter.WriteLine(string.Empty);
            }

            htmlWriter.RenderEndTag();    // </tr>
            htmlWriter.WriteLine(string.Empty);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a b l e R o w D a t a T e x t                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write report table row in text format.                             //
        // Data is supplied as a data row.                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void TableRowDataText(StreamWriter txtWriter,
                                              eRptChkMarks rptChkMarks,
                                              int colCt,
                                              string rowType,
                                              DataRow row,
                                              string[] colNames,
                                              int[] colSizes)
        {
            int lastCol = colCt - 1;

            string colSep = " ".PadRight(_lcSep, ' ');

            StringBuilder line = new StringBuilder();

            for (int i = 0; i < colCt; i++)
            {
                string itemData;

                if (row[colNames[i]] is bool)
                {
                    if ((bool)row[colNames[i]] == true)
                    {
                        if (rptChkMarks == eRptChkMarks.boxsym)
                            itemData = _chkMarkBoxSymTrue;
                        else if (rptChkMarks == eRptChkMarks.txtsym)
                            itemData = _chkMarkTxtSymTrue;
                        else
                            itemData = _chkMarkTextTrue;
                    }
                    else
                    {
                        if (rptChkMarks == eRptChkMarks.boxsym)
                            itemData = _chkMarkBoxSymFalse;
                        else if (rptChkMarks == eRptChkMarks.txtsym)
                            itemData = _chkMarkTxtSymFalse;
                        else
                            itemData = _chkMarkTextFalse;
                    }
                }
                else
                {
                    itemData = row[colNames[i]].ToString();
                }

                line.Append(itemData.PadRight(colSizes[i], ' '));

                if (i != lastCol)
                    line.Append(colSep);
            }

            txtWriter.WriteLine(line);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a b l e R o w D a t a X m l                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write report table row in xml format.                              //
        // Data is supplied as a data row.                                    //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void TableRowDataXml(XmlWriter xmlWriter,
                                              eRptChkMarks rptChkMarks,
                                             int colCt,
                                             string rowType,
                                             DataRow row,
                                             string[] colNames)
        {
            xmlWriter.WriteStartElement("item");

            if (rowType != null)
            {
                xmlWriter.WriteAttributeString("rowType", rowType);
            }

            for (int i = 0; i < colCt; i++)
            {
                xmlWriter.WriteStartElement(colNames[i].ToLower());

                if (row[colNames[i]] is bool)
                {
                    if ((bool)row[colNames[i]] == true)
                    {
                        if (rptChkMarks == eRptChkMarks.boxsym)
                            xmlWriter.WriteString(_chkMarkBoxSymTrue);
                        else if (rptChkMarks == eRptChkMarks.txtsym)
                            xmlWriter.WriteString(_chkMarkTxtSymTrue);
                        else
                            xmlWriter.WriteString(_chkMarkTextTrue);
                    }
                    else
                    {
                        if (rptChkMarks == eRptChkMarks.boxsym)
                            xmlWriter.WriteString(_chkMarkBoxSymFalse);
                        else if (rptChkMarks == eRptChkMarks.txtsym)
                            xmlWriter.WriteString(_chkMarkTxtSymFalse);
                        else
                            xmlWriter.WriteString(_chkMarkTextFalse);
                    }
                }
                else
                {
                    if ((i == 0) && (row[colNames[0]].ToString() == string.Empty))
                        xmlWriter.WriteCharEntity((char)0xa0);
                    else
                        xmlWriter.WriteString(row[colNames[i]].ToString());
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();   // </item>
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a b l e R o w P a i r                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write table row containing name and value components.              //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void TableRowPair(object writer,
                                         eRptFileFmt rptFileFmt,
                                         string txtName,
                                         string txtVal,
                                         int colSpanName,
                                         int colSpanVal,
                                         int sizeName,
                                         int sizeVal,
                                         bool blankBefore,
                                         bool blankAfter,
                                         bool nameAsHddr)
        {
            if (rptFileFmt == eRptFileFmt.html)
                TableRowPairHtml((HtmlTextWriter)writer,
                                  txtName, txtVal,
                                  colSpanName, colSpanVal,
                                  blankBefore, blankAfter, nameAsHddr);
            else if (rptFileFmt == eRptFileFmt.xml)
                TableRowPairXml((XmlWriter)writer, txtName, txtVal,
                                 colSpanName, colSpanVal,
                                 blankBefore, blankAfter, nameAsHddr);
            else
                TableRowPairText((StreamWriter)writer, txtName, txtVal,
                                  sizeName, sizeVal, blankBefore);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a b l e R o w P a i r H t m l                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write table row containing name and value components in html       //
        // format.                                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void TableRowPairHtml(HtmlTextWriter htmlWriter,
                                              string txtName,
                                              string txtVal,
                                              int colSpanName,
                                              int colSpanVal,
                                              bool blankBefore,
                                              bool blankAfter,
                                              bool nameAsHddr)
        {
            string padClass = string.Empty;

            htmlWriter.RenderBeginTag("tr");

            //----------------------------------------------------------------//

            if ((blankBefore) && (blankAfter))
                padClass = "padAntePost";
            else if (blankBefore)
                padClass = "padAnte";
            else if (blankAfter)
                padClass = "padPost";

            //----------------------------------------------------------------//

            if (padClass != string.Empty)
                if (nameAsHddr)
                    htmlWriter.AddAttribute("class",
                                             padClass + " " + "fmtAdorn");
                else
                    htmlWriter.AddAttribute("class", padClass);
            else if (nameAsHddr)
                htmlWriter.AddAttribute("class", "fmtAdorn");

            if (colSpanName != -1)
                htmlWriter.AddAttribute("colspan", colSpanName.ToString());

            htmlWriter.RenderBeginTag("td");

            htmlWriter.WriteEncodedText(txtName);
            htmlWriter.RenderEndTag();    // </td>

            htmlWriter.WriteLine(string.Empty);

            //----------------------------------------------------------------//

            if (padClass != string.Empty)
                htmlWriter.AddAttribute("class", padClass);

            if (colSpanName != -1)
                htmlWriter.AddAttribute("colspan", colSpanVal.ToString());

            htmlWriter.RenderBeginTag("td");

            htmlWriter.WriteEncodedText(txtVal);
            htmlWriter.RenderEndTag();    // </td>

            //----------------------------------------------------------------//

            htmlWriter.RenderEndTag();    // </tr>

            htmlWriter.WriteLine(string.Empty);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a b l e R o w P a i r T e x t                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write table row containing name and value components in text       //
        // format.                                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void TableRowPairText(StreamWriter txtWriter,
                                              string txtName,
                                              string txtVal,
                                              int maxSizeName,
                                              int maxSizeVal,
                                              bool blankBefore)
        {
            int valPos,
                  valLen;

            bool firstLine = true;

            if (blankBefore)
                txtWriter.WriteLine();

            valLen = txtVal.Length;
            valPos = 0;

            while (valPos + maxSizeVal < valLen)
            {
                string prefix;

                if (firstLine)
                    prefix = (txtName + ":").PadRight(maxSizeName, ' ');
                else
                    prefix = " ".PadRight(maxSizeName, ' ');

                txtWriter.WriteLine(
                        prefix +
                        txtVal.Substring(valPos, maxSizeVal));

                valPos += maxSizeVal;
                firstLine = false;
            }

            if (valPos <= valLen)
            {
                string prefix;

                if (firstLine)
                    prefix = (txtName + ":").PadRight(maxSizeName, ' ');
                else
                    prefix = " ".PadRight(maxSizeName, ' ');

                txtWriter.WriteLine(
                        prefix +
                        txtVal.Substring(valPos, valLen - valPos));
            }
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a b l e R o w P a i r X m l                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write table row containing name and value components in xml        //
        // format.                                                            //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void TableRowPairXml(XmlWriter xmlWriter,
                                             string txtName,
                                             string txtVal,
                                             int colSpanName,
                                             int colSpanVal,
                                             bool blankBefore,
                                             bool blankAfter,
                                             bool nameAsHddr)
        {
            string padClass = string.Empty;

            //----------------------------------------------------------------//

            if ((blankBefore) && (blankAfter))
                padClass = "padAntePost";
            else if (blankBefore)
                padClass = "padAnte";
            else if (blankAfter)
                padClass = "padPost";

            //----------------------------------------------------------------//

            xmlWriter.WriteStartElement("item");

            if (padClass != string.Empty)
                xmlWriter.WriteAttributeString("padType", padClass);

            xmlWriter.WriteStartElement("name");
            if (colSpanName != -1)
                xmlWriter.WriteAttributeString("colspan",
                                                colSpanName.ToString());
            if (nameAsHddr)
                xmlWriter.WriteAttributeString("txtfmt",
                                                "fmtAdorn");

            xmlWriter.WriteString(txtName);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("value");
            if (colSpanName != -1)
                xmlWriter.WriteAttributeString("colspan",
                                                colSpanVal.ToString());
            xmlWriter.WriteString(txtVal);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();   // </item>
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a b l e R o w T e x t                                            //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write report table row from data supplied as a string array.       //
        //                                                                    //
        //--------------------------------------------------------------------//

        public static void TableRowText(object writer,
                                         eRptFileFmt rptFileFmt,
                                         int colCt,
                                         string[] data,
                                         string[] colNames,
                                         int[] colSizes)
        {
            if (rptFileFmt == eRptFileFmt.html)
                TableRowTextHtml((HtmlTextWriter)writer, colCt, data);
            else if (rptFileFmt == eRptFileFmt.xml)
                TableRowTextXml((XmlWriter)writer, colCt,
                              data, colNames);
            else
                TableRowTextText((StreamWriter)writer, colCt, data, colSizes);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a b l e R o w T e x t H t m l                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write report table row in html format.                             //
        // Data is supplied as a string array.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void TableRowTextHtml(HtmlTextWriter htmlWriter,
                                              int colCt,
                                              string[] data)
        {
            int lastCol = colCt - 1;

            htmlWriter.RenderBeginTag("tr");

            for (int i = 0; i < colCt; i++)
            {
                htmlWriter.RenderBeginTag("td");
                if ((i == 0) && (data[0] == string.Empty))
                    htmlWriter.Write("&nbsp;");
                else
                    htmlWriter.WriteEncodedText(data[i]);
                htmlWriter.RenderEndTag();    // </td>

                if (i != lastCol)
                    htmlWriter.WriteLine(string.Empty);
            }

            htmlWriter.RenderEndTag();    // </tr>
            htmlWriter.WriteLine(string.Empty);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a b l e R o w T e x t T e x t                                    //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write report table row in text format.                             //
        // Data is supplied as a string array.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void TableRowTextText(StreamWriter txtWriter,
                                              int colCt,
                                              string[] data,
                                              int[] colSizes)
        {
            int lastCol = colCt - 1;

            string colSep = " ".PadRight(_lcSep, ' ');

            StringBuilder line = new StringBuilder();

            for (int i = 0; i < colCt; i++)
            {
                line.Append((data[i]).PadRight(colSizes[i], ' '));

                if (i != lastCol)
                    line.Append(colSep);
            }

            txtWriter.WriteLine(line);
        }

        //--------------------------------------------------------------------//
        //                                                        M e t h o d //
        // t a b l e R o w T e x t X m l                                      //
        //--------------------------------------------------------------------//
        //                                                                    //
        // Write report table row in xml format.                              //
        // Data is supplied as a string array.                                //
        //                                                                    //
        //--------------------------------------------------------------------//

        private static void TableRowTextXml(XmlWriter xmlWriter,
                                             int colCt,
                                             string[] data,
                                             string[] colNames)
        {
            xmlWriter.WriteStartElement("item");

            for (int i = 0; i < colCt; i++)
            {
                xmlWriter.WriteStartElement(colNames[i].ToLower());
                if ((i == 0) && (data[0] == string.Empty))
                    xmlWriter.WriteCharEntity((char)0xa0);
                else
                    xmlWriter.WriteString(data[i]);
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();   // </tableitem>
        }
    }
}