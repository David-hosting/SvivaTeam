SvivaTeam
=========

_**"A Better World Is Made Together"**_

## How To Download ##
- **Step No.1** - Making sure we have:<br>
  - A. Visual Studio
  - B. SQL Server Management Studio
  - Git & Git Bash
  If you do not have one or any of them you can download them here:<br>
  [Download Visual Studio download link][1]<br>
  [Download SQL Server Management Studio][2]<br>
  [Download Git & Git Bash][3]
  * _Note: make sure you downloaded the `Data storage and processing` tool in visual studio_
- **Step No.2** - Creating / Opening a folder where the project will be downloaded:<br>
  You can create a folder or open (preferably) an empty folder
- **Stem No.3** - Cloning the repo to desktop:<br>
  After you have opened your folder double click on the path (where it is empty) and wirte in it `cmd`.<br>
  It should look like this:<br>
  ![image](https://user-images.githubusercontent.com/67858186/225978346-4a03f0f6-393f-441c-ae4a-3e6766f0b1d1.png)
  After this, click enter and a `cmd` console will open up, then clone the project to the folder using this command: `git clone https://github.com/David-hosting/SvivaTeam`<br>
  If this is what you are getting then you are good to go:<br>
  ![image](https://user-images.githubusercontent.com/67858186/225978734-1c2d8e24-f1c1-493f-ac7b-282c08995268.png)
  - Double-click on the connection string's value (what its in green) and copy it.
- **Step No.4** - Restoring the `.bak` files:<br>
  The `.bak` files are backups of the SQL databases. In order to restore them we will do:
  - A. Open SQL Server Management Studio
  - B. Select your server _be aware that you may have more than one SQL server, so choose the server you to store the databases in_ and connect.<br>
   ![צילום מסך 2023-03-17 200638](https://user-images.githubusercontent.com/67858186/225984836-3fe89a42-170b-499d-953d-f47f70bda18b.png)

  - C. Open the server _To open or duble click on it or use the (+) button_.
  - D. right click on the `Databases` folder and select `Restore Database...` <br>
  ![צילום מסך 2023-03-17 201514](https://user-images.githubusercontent.com/67858186/225985950-d828f930-0c9a-43ed-a5d3-78477194ebe0.png)
  - E. A window will pop up, change the Source to `Device` and then on the `[...]` button.<br>
  ![צילום מסך 2023-03-17 201751](https://user-images.githubusercontent.com/67858186/225986500-8d646dcd-cca3-416c-a809-168b29cb275b.png)
  - F. Anoter window will pop up, click on `Add`.<br>
    ![צילום מסך 2023-03-17 202222](https://user-images.githubusercontent.com/67858186/225987369-2452d1f2-87a3-4286-936e-1e768fdbb0d9.png)
  - G. One more window will pop up, navigate to where the solution is stored, there will be a folder called `Databases Backup`, open it and copy its path. Paste it in the new window's path then select one of the `.bak` files and click `OK`.<br>
  ![צילום מסך 2023-03-17 203011](https://user-images.githubusercontent.com/67858186/225988794-2498ee35-c4a7-4ec9-8e10-897c87e9ca48.png)
  - H. Click `OK` again.<br>
  ![צילום מסך 2023-03-17 203327](https://user-images.githubusercontent.com/67858186/225991283-0796a14f-58e6-4ee5-8c12-551fa356b24f.png)
  - I. Click `OK` again.<br>
    ![צילום מסך 2023-03-17 204258](https://user-images.githubusercontent.com/67858186/225991434-f6e8bd2b-489c-4ab5-9dcf-f44bbf439761.png)
  - J. Repeat stages `D. to I.` and recover the second file.
- **Step No.6** - Change the SQL database path to your path
  - Starting from the `AuthDb` database, 

    
  
 

  [1]: https://visualstudio.microsoft.com/downloads/
  [2]: https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms?view=sql-server-ver16
  [3]: https://git-scm.com/downloads
