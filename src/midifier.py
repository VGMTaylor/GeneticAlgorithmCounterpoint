from midiutil.MidiFile import MIDIFile

with open("Cantusfirmus.txt") as myfile:
    data="".join(line.rstrip() for line in myfile)
	
with open("GeneratedCM.txt") as myfile2:
    cmdata="".join(line.rstrip() for line in myfile2)	

pitches_list = []
cantusfirmus = []
countermelody = []
i = 1
firstnum = ''
secondnum = ''
for c in data:
	if (i % 2 == 0):
		secondnum = str(c)
		cantusfirmus.append(firstnum + secondnum)
	else:
		firstnum = str(c)
	i += 1
	
j = 1
firstnum2 = ''
secondnum2 = ''
for d in cmdata:
	if (j % 2 == 0):
		secondnum2 = str(d)
		countermelody.append(firstnum2 + secondnum2)
	else:
		firstnum2 = str(d)
	j += 1	

#print(cantusfirmus)
#print(countermelody)

countermelody2 = ['33', '28', '35', '29', '43', '40', '38', '36', '33', '31', '33']
track    = 0
channel  = 0
time     = 1   # In beats
duration = 1   # In beats
tempo    = 30  # In BPM
volume   = 100 # 0-127, as per the MIDI standard

MyMIDI = MIDIFile(1) # One track, defaults to format 1 (tempo track # automatically created)
MyMIDI.addTempo(track,time, tempo)

ij = 0
ji = 0

print("\n")

# Ask for the number and store it in userNumber
userNumber = input('Which species? ')

# Make sure the input is an integer number
userNumber = int(userNumber)

if(userNumber == 1):
	for pitch in countermelody:
		if(ij < (len(countermelody))):
			MyMIDI.addNote(track, 0, int(cantusfirmus[ij]) + 12, time, duration, 80)
			MyMIDI.addNote(track, 0, int(countermelody[ij]) + 12, time, duration, 70)
			time = time + 1
			ij += 1
	with open("species1.mid", "wb") as output_file:
		MyMIDI.writeFile(output_file)
			
if(userNumber == 2):
	for pitch in countermelody:
		if(ji < 1):
			MyMIDI.addNote(track, 0, int(cantusfirmus[ij]) + 12, time, duration, 80)
			MyMIDI.addNote(track, 0, int(countermelody[ji]) + 12, time + 0.5, duration / 2, 70)
			ji += 1
			time = time + 1;
			ij += 1			
		elif(ji < (len(countermelody) - 1)):
			MyMIDI.addNote(track, 0, int(cantusfirmus[ij]) + 12, time, duration, 80)
			MyMIDI.addNote(track, 0, int(countermelody[ji]) + 12, time, duration / 2, 70)
			ji += 1
			MyMIDI.addNote(track, 0, int(countermelody[ji]) + 12, time + 0.5, duration / 2, 50)
			time = time + 1;
			ji += 1
			ij += 1
		else:
			MyMIDI.addNote(track, 0, int(cantusfirmus[ij]) + 12, time, duration, 80)
			MyMIDI.addNote(track, 0, int(countermelody[ji]) + 12, time, duration, 70)
	with open("species2.mid", "wb") as output_file:
		MyMIDI.writeFile(output_file)	
		
if(userNumber == 3):
	for pitch in countermelody:
		if(ji < 1):
			MyMIDI.addNote(track, 0, int(cantusfirmus[ij]) + 12, time, duration, 80)
			MyMIDI.addNote(track, 0, int(countermelody[ji]) + 12, time + 0.25, duration / 4, 70)
			ji += 1
			MyMIDI.addNote(track, 0, int(countermelody[ji]) + 12, time + 0.5, duration / 4, 70)
			ji += 1
			MyMIDI.addNote(track, 0, int(countermelody[ji]) + 12, time + 0.75, duration / 4, 70)
			time = time + 1;
			ji += 1
			ij += 1	
		if(ij < (len(cantusfirmus) - 1)):
			MyMIDI.addNote(track, 0, int(cantusfirmus[ij]) + 12, time, duration, 80)
			MyMIDI.addNote(track, 0, int(countermelody[ji]) + 12, time, duration / 4, 70)
			ji += 1
			MyMIDI.addNote(track, 0, int(countermelody[ji]) + 12, time + 0.25, duration / 4, 55)
			ji += 1
			MyMIDI.addNote(track, 0, int(countermelody[ji]) + 12, time + 0.50, duration / 4, 60)
			ji += 1
			MyMIDI.addNote(track, 0, int(countermelody[ji]) + 12, time + 0.75, duration / 4, 50)
			time = time + 1;
			ji += 1
			ij += 1		
		else:
			MyMIDI.addNote(track, 0, int(cantusfirmus[ij]) + 12, time, duration, 80)
			MyMIDI.addNote(track, 0, int(countermelody[ji]) + 12, time, duration, 70)
	with open("species3.mid", "wb") as output_file:
		MyMIDI.writeFile(output_file)

if(userNumber == 4):
	for pitch in countermelody:
		if(ij < (len(countermelody) - 1)):
			MyMIDI.addNote(track, 0, int(cantusfirmus[ij]) + 12, time, duration, 80)
			MyMIDI.addNote(track, 0, int(countermelody[ij]) + 12, time + 0.5, duration, 70)
			time = time + 1
			ij += 1
		else:
			MyMIDI.addNote(track, 0, int(cantusfirmus[ij]) + 12, time, duration, 80)
			MyMIDI.addNote(track, 0, int(countermelody[ij]) + 12, time, duration, 70)
	with open("species4.mid", "wb") as output_file:
		MyMIDI.writeFile(output_file)			
			
# with open("major-scale.mid", "wb") as output_file:
	# MyMIDI.writeFile(output_file)

# with open("major-scale.mid", "wb") as output_file:
	# MyMIDI.writeFile(output_file)
