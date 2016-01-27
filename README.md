# find-trash-files
<span>Allows you to find duplicated / empty files on a folder and creates a report of those files</span>
<span>This was created during a innovation time, so there's tons of space for improvements!</span>
<h2> How to use it? </h2>

<table>
    <tbody>
        <tr>
        <td>
            <b>Simple Usage :</b>
        </td>
        <td>
            <code>
                find-trash-files.exe "inputFolder" "outputLocation"
            </code>
        </td>
        </tr>
        <tr>
        <td>
            <b>Advanced Usage :</b>
        </td>
        <td>
            <h4>Disable UI:</h4>
            <code>
                find-trash-files.exe "inputFolder" "outputLocation" -disableUI
            </code>
            <h4>Servermode (instead of an excel format the ouput is a txt):</h4>
            <code>
                find-trash-files.exe "inputFolder" "outputLocation" -servermode
            </code>
            <h4>Filter (allows you to filter file formats / folders):</h4>
            <code>
                find-trash-files.exe "inputFolder" "outputLocation" -filter ".js,folderBlalala,.cake"
            </code>
        </td>
        </tr>
        <tr>
        <td>
            <b>Example :</b>
        </td>
        <td>
            <code>
                find-trash-files.exe "C:\Projects\projectFolder" "C:\Projects"
            </code>
        </td>
        </tr>
    </tbody>
</table>

<h2> What do the symbols / colors mean? </h2>
<h3>There are 3 phases</h3>
<h5>Phase 1 - Finding Files and pre-checking</h5>
<span>The program is detecting the files in the inputed folder and creating the data to later on parse them</span>
<span>After that is complete, it will detect possible duplicated files</span>
<table>
    <tbody>
        <tr>
            <td>
                <span>=</span>
            </td>
            <td>
                File is not duplicated / empty (White)
            </td>
        </tr>
        <tr>
            <td>
                <img src="http://i.imgur.com/GIYFtm6.png"/>
            </td>
            <td>
                Duplicated file found (Red)
            </td>
        </tr>
        <tr>
            <td>
                <img src="http://i.imgur.com/ip6MVyY.png"/>
            </td>
            <td>
                Duplicated image file found (Dark red)
            </td>
        </tr>
    </tbody>
</table>
<h5>Phase 2 - Comparison</h5>
<span>The program will now compare the current file with the all the possible duplicates</span>
<table>
    <tbody>
        <tr>
            <td>
                <img src="http://i.imgur.com/rI5YvzR.png"/>
            </td>
            <td>
                Compared duplicated files (Purple)
            </td>
        </tr>
    </tbody>
</table>
<h5>Phase 3 - Cleanup</h5>
<span>In this phase, the program will prepare it self to generate the excel / text output</span>
<table>
    <tbody>
        <tr>
            <td>
                <img src="http://i.imgur.com/J3o9bA1.png"/>
            </td>
            <td>
                File ready for output (Green)
            </td>
        </tr>
        <tr>
            <td>
                <img src="http://i.imgur.com/KF0ipM6.png"/>
            </td>
            <td>
                Image ready for output (Red with !!)
            </td>
        </tr>
    </tbody>
</table>
<h2>How can i contribute?</h2>
<span>Easy! Fork the project, add your changes / improvements and create a pull request!</span>
<h2>Screenies</h2>
<img src="http://i.imgur.com/VyOM6Qa.png" />
<img src="http://i.imgur.com/8UsK4Qn.png" />
<img src="http://i.imgur.com/baRZp2z.png" />
