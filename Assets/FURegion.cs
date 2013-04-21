using UnityEngine;
using System;
using System.Collections;


public class FURegion {
	public readonly Color32[] pixels;
	public uint[] indexedPixels;
	
	public readonly int width;
	public readonly int height;
	
	public void FitPalette( Color32[] palette ) {
		// try to index colors using the supplied palette; non-matching colors are not assigned
		this.indexedPixels = new uint[ this.width * this.height ];
		for( int i = 0; i < this.height; i++ ) {
			for( int j = 0; j < this.width; j++ ) {
				Color32 c = this.pixels[ i * this.width + j ];
				
				this.indexedPixels[ i * this.width + j ] = uint.MaxValue;
				for( int k = 0; k < palette.Length; k++ ) {
					if( c.r == palette[k].r && c.g == palette[k].g && c.b == palette[k].b ) {
						this.indexedPixels[ i * this.width + j ] = (uint)k;
						break;
					}
				}
			}
		}
	}
		
	public FURegion( PaletteFupixel fromContext, int ul_x, int ul_y, int width, int height ) {	
		this.pixels = new Color32[ width * height ];
		if( fromContext.indexedPixels != null ) {
			this.indexedPixels = new uint[ width * height ];
		}
		
		this.width = width;
		this.height = height;
		
		for( int i = ul_y; i < ul_y + height; i++ ) {
			for( int j = ul_x; j < ul_x + width; j++ ) {
				this.pixels[ (i - ul_y) * this.width + (j - ul_x) ] = fromContext.pixels[ i * fromContext.width + j ];
				if( fromContext.indexedPixels != null ) {
					this.indexedPixels[ (i - ul_y) * this.width + (j - ul_x) ] = fromContext.indexedPixels[ i * fromContext.width + j ];
				}
			}
		}	
	}
	
	public FURegion( FURegion fromRegion, int ul_x, int ul_y, int width, int height ) {	
		this.pixels = new Color32[ width * height ];
		if( fromRegion.indexedPixels != null ) {
			this.indexedPixels = new uint[ width * height ];
		}
		
		this.width = width;
		this.height = height;
		
		for( int i = ul_y; i < ul_y + height; i++ ) {
			for( int j = ul_x; j < ul_x + width; j++ ) {
				this.pixels[ (i - ul_y) * this.width + (j - ul_x) ] = fromRegion.pixels[ i * fromRegion.width + j ];
				if( fromRegion.indexedPixels != null ) {
					this.indexedPixels[ (i - ul_y) * this.width + (j - ul_x) ] = fromRegion.indexedPixels[ i * fromRegion.width + j ];
				}
			}
		}
	}
	
	public FURegion( Texture2D fromTexture ) {
		this.pixels = fromTexture.GetPixels32();
		this.indexedPixels = null;
		this.width = fromTexture.width;
		this.height = fromTexture.height;
	}
}

