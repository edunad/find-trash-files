# find-trash-files
Allows you to find duplicated / empty files on a folder and creates a report of those files

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
                File is not duplicated / empty
            </td>
        </tr>
        <tr>
            <td>
                <span style="color:#FF0000;">#</span>
            </td>
            <td>
                Duplicated file found
            </td>
        </tr>
        <tr>
            <td>
                <span style="color:#FF0000;">#</span>
            </td>
            <td>
                Duplicated image file found
            </td>
        </tr>
    </tbody>
</table>
