<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" xmlns:dc="http://purl.org/dc/elements/1.1/" version="1.0">
<xsl:output method="xml"  />
<xsl:template match="/">
<html xmlns="http://www.w3.org/1999/xhtml" lang="en" xml:lang="en">
<head>
<title><xsl:value-of select="rss/channel/title"/></title>
<style type="text/css">
</style>
</head>
<body xmlns="http://www.w3.org/1999/xhtml">
	<div id="ContentDescription">
		<h1>What is RSS?</h1>
		<p>	
	        This page is an RSS syndication feed. Simply put, it's a common technique used for sharing content on the web for reuse.
	        </p>
		<p>
		If you are not familiar with what RSS is and would like to learn more visit <a target="_blank" href="http://en.wikipedia.org/wiki/RSS_%28file_format%29">Wikipedia</a> for an explanation.
		</p>
        	<p>
		Using this web address (URL) you can use content from this site in a variety of tools and websites. For individuals, the most likely usage 
		is within an application known as an RSS reader. An RSS reader is an application you run on your own computer which can request and store the 
	        content from RSS feeds.
		</p>
                <p>
		To use this feed in your RSS reader software you will need the web address (URL) of this page. Just copy and paste it where your RSS Reader software asks for a URL.
		</p>
	</div>
	
	<div id="Content">
		<h1><xsl:value-of select="rss/channel/title"/></h1>
		
		<ol id="ItemList">
			<xsl:for-each select="rss/channel/item">       
				<li class="ItemListItem">
					<h2><a><xsl:attribute name="href"><xsl:value-of select="link"/></xsl:attribute><xsl:value-of select="title"/></a></h2>
					<pre><xsl:value-of select="description"/></pre>
					<div class="ItemListItemDetails">Published <xsl:value-of select="pubDate"/></div>
				</li>
			</xsl:for-each>
		</ol>
	</div>
	
</body>
</html>

</xsl:template>
</xsl:stylesheet>