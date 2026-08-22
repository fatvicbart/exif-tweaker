@echo off
SET /p Path=Chemin du repertoire des photos a traiter: 
exiftool "-alldates<${filename;s/_.*//} 000000" %Path% -overwrite_original -r
pause