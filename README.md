<p align="center">
  <img src="http://i.imgur.com/WPpSxPE.png"/>
  <h2 align="center">Strategy Incubator</h2>
</p>

Strategy Incubator is an *RSS* and *Discord* bot developed for www.merrygraverobbers.com.
Its purpose is to frequently check the rss feed of a specific part of the vBulletin forum section and relay
to our Discord server if any new posts or threads has been made. This was developed to make it easier
for members to see when a new post has been made so that they can read it and provide feedback quicker.

--

```
Settings.json
{
  "tasks": [
    {
      "rss": "http://example.com/external?nodeid=10&type=rss2",
      "channelid": 219919180525207552,
      "discordmsg": "*Author: {0} Post title: {1} Forum link: {2} Post summary {3}"
    },
    {
      "rss": "http://example.com/external?nodeid=20&type=rss2",
      "channelid": 245028729594707968,
      "discordmsg": "*Author: {0} Post title: {1} Forum link: {2} Post summary {3}"
    }
  ],
  "intervalMinutes": 5, -- How often rss tasks will be queried (minutes)
  "appname": "StrategyIncubator", -- Discord app name
  "game": "RSS", -- Discord playing game label
  "token": "ABC123" -- Discord bot account access token
}
```
