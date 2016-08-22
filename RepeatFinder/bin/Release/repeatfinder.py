 # -*- coding: utf-8 -*-
 # Author: Sokolov Denis (C) 2016

import platform
import sys
from docx import Document
from difflib import SequenceMatcher

if platform.system() == 'Windows':
	reload(sys)
	sys.setdefaultencoding('cp866')

if len(sys.argv)==1:
    print u"Использование: python repeatfinder.py word_document_name"
    sys.exit(1)
    
text = ""
min_ratio = 0.80
maxlength = 10

print u"Чтение аргументов командной строки..."
if len(sys.argv)>2:
	for a in sys.argv:
		if '%' in a:
			min_ratio = float(a.replace('%',''))/100.0
		if '#' in a:
			maxlength = int(a.replace('#',''))			

print sys.argv[1]
print "min_ratio=%f maxlength=%d" % (min_ratio, maxlength)

document = Document(sys.argv[1])
print u"Чтение документа, ждите..."
for paragraph in document.paragraphs:
    text += paragraph.text
print ""
    
spl_text = text.split('.')

full = open("full_match.txt","w")
part = open("part_match.txt","w")
matches = []

print u"Анализ текста, ждите..."
print ""

for i in xrange(0,len(spl_text)):
    for j in xrange(i+1, len(spl_text)):
        str1 = spl_text[i].strip()
        str2 = spl_text[j].strip()
        s = SequenceMatcher(None, str1, str2)
        ratio = s.ratio()
        
        if ratio >= min_ratio and len(str1)>maxlength:
            match = (str1, str2)
            if match not in matches:
                matches.append(match)                
                if ratio == 1.0:
                    print u"Обнаружено полное совпадение:"
                    print "I:  " + str1.encode('UTF-8')
                    print "II: " + str2.encode('UTF-8')
                    print ''
                    full.writelines("I:  " + str1.encode('UTF-8') +"\n")
                    full.writelines("II: " + str2.encode('UTF-8') +"\n")
                    full.writelines("\n")
                else:
                    print u"Обнаружено совпадение на " + str(ratio*100).encode('UTF-8') + "%" 
                    print "I:  " + str1.encode('UTF-8')
                    print "II: " + str2.encode('UTF-8')
                    print ''
                    part.writelines("Совпадение на " + str(ratio*100).encode('UTF-8') + "%\n" )
                    part.writelines("I:  " + str1.encode('UTF-8') +"\n")
                    part.writelines("II: " + str2.encode('UTF-8') +"\n")
                    part.writelines("\n")                    
            
full.close()
part.close()

print u"Анализ текста завершён..."
print u"Отчёт сохранён в файлы:"
print u"full_match.txt - полные совпадения"
print u"part_match.txt - частичные совпадения"


            
        
    



    
    
    