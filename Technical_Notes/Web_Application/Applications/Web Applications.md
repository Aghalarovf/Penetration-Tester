# Common Applications
```powershell

```
<img width="938" height="784" alt="image" src="https://github.com/user-attachments/assets/20045431-31e4-4494-92b3-58dae24f33db" />
<img width="935" height="577" alt="image" src="https://github.com/user-attachments/assets/ec56e8f0-bb5e-4496-b3cd-39dc9d74d5dd" />

# Web Application Discovery
```powershell
scope_list:
app.inlanefreight.local
dev.inlanefreight.local
drupal-dev.inlanefreight.local
drupal-qa.inlanefreight.local
drupal-acc.inlanefreight.local
drupal.inlanefreight.local
blog-dev.inlanefreight.local
blog.inlanefreight.local
app-dev.inlanefreight.local
jenkins-dev.inlanefreight.local
jenkins.inlanefreight.local
web01.inlanefreight.local
gitlab-dev.inlanefreight.local
gitlab.inlanefreight.local
support-dev.inlanefreight.local
support.inlanefreight.local
inlanefreight.local
10.129.201.50


nmap -p 80,443,8000,8080,8180,8888,10000 --open -oA web_discovery -iL scope_list
```

# EyeWitness
```powershell
sudo apt install eyewitness

eyewitness --web -x web_discovery.xml -d inlanefreight_eyewitness
```

# Aquatone
```powershell
wget https://github.com/michenriksen/aquatone/releases/download/v1.7.0/aquatone_linux_amd64_1.7.0.zip

unzip aquatone_linux_amd64_1.7.0.zip

cat web_discovery.xml | ./aquatone -nmap
```


