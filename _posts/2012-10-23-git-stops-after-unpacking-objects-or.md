---
layout: post
title: GIT stops after unpacking objects or resolving deltas
date: '2012-10-23T15:32:00.003-07:00'
author: Jan Fajfr
tags:
- Source Control
modified_time: '2014-06-26T14:15:06.778-07:00'
blogger_id: tag:blogger.com,1999:blog-1710034134179566048.post-4011874078382089830
blogger_orig_url: http://hoonzis.blogspot.com/2012/10/git-stops-after-unpacking-objects-or.html
---
This post describes an issues that you can get while using GIT behind NTLM (NT Lan Manager protocol) proxy, often used in corporations with windows networks.

Clone, pull and push all seem to finish fine, but they always stop at "Resolving deltas" or "Unpacking objects". The root of this issue is the fact that MSGIT uses curl, which does not know how to talk to NTLM authenticated proxy. There are two solutions which worked for me, first one not being stable.

### Solution 1: unstable, easiest

This \*usually\* works:

``` 
git clone -v https://myrepo.git
Cloning into 'myrepo-1'...
Password for 'https://user@myrepo.com':
POST git-upload-pack (174 bytes)
remote: Counting objects: 2287, done.
remote: Compressing objects: 100% (1276/1276), done.
remote: Total 2287 (delta 1340), reused 1692 (delta 883)
Receiving objects: 100% (2287/2287), 53.37 MiB | 632 KiB/s, done.
Resolving deltas: 100% (1340/1340), done.
```

Or another when, when pulling from a repo:

``` 
remote: Counting objects: 53, done.
remote: Compressing objects: 100% (28/28), done.
remote: Total 28 (delta 23), reused 2 (delta 0)
Unpacking objects: 100% (28/28), done.
```

The problem is that nothing is updated on the disk. I have tried using
**-v** in order to obtain more information about what is going on, but I
never get more than shown above. I am using Git over HTTPS with the
HTTPS proxy well configured - so this should not be an issue.

After un-finished clone, I have noticed, that the **objects** directory
inside the **.git** directory actually changed it's size quite a lot. So
it seems, that the objects are well received but never written to the
actual branch. That might sound like a user-rights issue. But I am admin
on the machine and I have access write rights on the folder. So for now,
I don't know the cause of this problem. I have simple solution to
overcome this issue. What I do to fix the repository is quite
straightforward:


``` 
$ git fsck
Checking object directories: 100% (256/256), done.
Checking objects: 100% (2187/2187), done.
dangling commit da575f887db63ccf97f37f9cf96316307398db82
```

**Git fsck** always founds one dangling commit. It is a utility which
checks the integrity of the object database. Here the dangling commit is
a commit which is not used by any branch. To fix the issue I can just
merge the commit with the current branch.

    $git merge da575f887db63ccf97f37f9cf96316307398db82

And everything works just fine...

### Solution 2: stable solution

This needs some more effort. The solution is to use the CNTLM as a proxy
between the NTLM proxy and any application which needs to use it. [The
solution is excellently described on this
blog.](http://sparethought.wordpress.com/2012/12/06/setting-git-to-work-behind-ntlm-authenticated-proxy-cntlm-to-the-rescue/)

### Solution 3: not confirmed

I was not able to confirm this one, but at [this thread on StackOverflow](http://stackoverflow.com/questions/13473341/git-clone-with-ntlm-proxy-hangs-after-resolving-deltas)
someone mentioned to use **verify-pack** function of git. Verify-pack
verifies that *.pack* files, which are packed objects used by git during
the transfer. If the files were transfered correctly you should be able
to restore the commits contained inside the pack files. This actually
did not work on my post, since the pack file was corrupted.
