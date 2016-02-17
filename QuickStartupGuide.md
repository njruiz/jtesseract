# Quick Start up Guide #
JTesseract uses _tes_ file extension for storing its project files.

## Step 1 ##
Open JTesseract.exe

## Step 2 ##
First make sure you have copied **all** tesseract binary files in to a known location. Next let JTesseract know the directory by using menu **Tools->Options**

## Step 3 ##
Use menu **File->New Project**
  * JTesseract will prompt a "Project Settings" dialog to ask a language name. This is the **3 letter language code** you are going to use for the language pack. The default "eng" refers to the English language.
  * JTesseract next prompts a "Save As" dialog to save the project. Feel free to navigate to your preferred directory and provide a **valid project name**. Just remember it will create a sub folder and import all the training files in to it.
i.e. If you provide C:\my\_tess\srilanka.tes, JTesseract will create C:\my\_tess\srilanka\ sub folder and store all the images and intermediate files in it.

## Step 4 ##
Under "Project Assistant" tab,
  * Click on **"1. Load Images..."** button. You will be prompted with a "File Open" dialog box.
  * Now **select all the training images** you want to use for the training. At the end click on "Open" button.
JTesseract now imports all the images to the project folder and create corresponding box files.

## Step 5 ##
Use "Source Images" tab for box file editing.
You can see the list of images imported in the top left corner of the "Source Images" tab.
You can **click on each image** to proceed to **box file editing.**

Use mouse wheel to zoom in and zoom out in the main window.

Click on navigation buttons to navigate character by character.

Once you find a faulty reading, correct them and click on "Apply Changes" button, found just under navigation buttons.

Once you are done editing box files corresponding to all images proceed to step 6.

## Step 6 ##
Use "Project Assistant" tab, and click on **"2. Generate Training Data Set & Cluster..."** button.
Now JTesseract will create corresponding _tr_ files and perform clustering. Keep looking at the Tesseract.log window for the result of the Tesseract.log. If you find a problem in the Tesseract.log you may have to go back to step 5.

Please have a look at http://code.google.com/p/tesseract-ocr/wiki/TrainingTesseract if you have any doubt on the errors getting in the Tesseract.log window.

## Step 7 ##
Now it is time to deal with words.
Use "Words/Characters" tab to add words/frequent words/user-words etc..
Please note that you have to input one word per each line. If you are done with adding words, go back to "Project Assistant" tab and click on **"3. Generate Words and Character Lists..."** button.

Remember this step will take a long time and JTesseract will stop responding until that process completes. (Yeah I know, I should have fixed it, will do soon :P)

## Step 8 ##
Now lets click on **"4. Save Language Pack..."** button.
Again JTesseract will prompt you with a language code (in case you have changed your mind..). Once you press OK, JTesseract will save the language pack under <your project location>\language\_pack\ folder.
i.e. If you saved this project at C:\my\_tess\srilanka.tes, you can find the language pack at C:\my\_tess\srilanka\language\_pack\ folder.

### If everything goes smoothly, Congratulations you are **DONE!** ###



Remember you can go back to any of the steps above and re-do everything any number of time.

## Next ##
You can use the language pack with tesseract to OCR the language you have just trained.

I would recomment using following GUI application for the process.
http://softi.co.uk/freeocr.htm (copy the language pack to <FreeOCR installed folder>\tesseract\tessdata\ and select the language from the top-right corner after restarting the application)

## How about? ##
Now if you are free, how about writing a nice/better user guide.