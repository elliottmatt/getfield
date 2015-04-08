# getfield
Simple "cut"-like tool for working with field-delimited flat files

Example Usage:
getfield -f 1,2,3,4-10 file.txt -d "|"
(same as above) getfield -1,2,3,4-10 file.txt
(same as above) cat file.txt | getfield -1,2,3,4-10

Useful for changing the delimiter of a file:
(preserving quotes, change csv to pipes)
getfield -* file.txt -d "," -Q -od "|"
