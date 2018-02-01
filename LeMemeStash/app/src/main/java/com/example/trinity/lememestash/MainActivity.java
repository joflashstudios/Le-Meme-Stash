package com.example.trinity.lememestash;

import android.Manifest;
import android.annotation.SuppressLint;
import android.content.Intent;
import android.database.Cursor;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.media.ThumbnailUtils;
import android.net.Uri;
import android.os.AsyncTask;
import android.os.Bundle;
import android.os.Environment;
import android.os.ParcelFileDescriptor;
import android.provider.MediaStore;
import android.support.design.widget.FloatingActionButton;
import android.support.v4.app.ActivityCompat;
import android.support.v7.app.AppCompatActivity;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.GridView;
import android.widget.ImageView;

import java.io.Console;
import java.io.File;
import java.io.FileDescriptor;
import java.io.IOException;
import java.util.ArrayList;
import java.util.List;
import java.util.logging.ConsoleHandler;

public class MainActivity extends AppCompatActivity {
    private static final int REQUEST_MEME_GET = 420;
    private static final int REQUEST_READ_EXTERNAL_STORAGE = 666;

    private GridView memeGrid;
    private MemeAdapter memeAdapter;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        memeAdapter = new MemeAdapter(getBaseContext(), 0);

        FloatingActionButton fab = findViewById(R.id.fab);
        memeGrid = findViewById(R.id.meme_grid);
        memeGrid.setAdapter(memeAdapter);

        fab.setOnClickListener(view -> {
            ActivityCompat.requestPermissions(this, new String[]{Manifest.permission.READ_EXTERNAL_STORAGE}, REQUEST_READ_EXTERNAL_STORAGE);
            Intent open = new Intent(Intent.ACTION_GET_CONTENT);
            open.setType("*/*");
            if (open.resolveActivity(getPackageManager()) != null) {
                startActivityForResult(open, REQUEST_MEME_GET);
            }
        });
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (requestCode == REQUEST_MEME_GET && resultCode == RESULT_OK) {
            Uri fullPhotoUri = data.getData();
            handleMemeAddition(fullPhotoUri);
        }
    }

    private void handleMemeAddition(Uri imageUri) {
        @SuppressLint("StaticFieldLeak")
        AsyncTask<Void, Void, Bitmap> task = new AsyncTask<Void, Void, Bitmap>() {
            @Override
            protected Bitmap doInBackground(Void... uris) {
                try {
                    return getBitmapFromUri(imageUri);
                } catch (IOException e) {
                    return null;
                }
            }

            @Override
            protected void onPostExecute(Bitmap result) {
                Bitmap resized = ThumbnailUtils.extractThumbnail(result, 500, 500);
                Meme newMeme = new Meme();
                newMeme.thumbnail = resized;
                memeAdapter.add(newMeme);
            }
        };

        task.execute();
    }

    private Bitmap getBitmapFromUri(Uri uri) throws IOException {
        ParcelFileDescriptor parcelFileDescriptor =
                getContentResolver().openFileDescriptor(uri, "r");
        FileDescriptor fileDescriptor = parcelFileDescriptor.getFileDescriptor();
        Bitmap image = BitmapFactory.decodeFileDescriptor(fileDescriptor);
        parcelFileDescriptor.close();
        return image;
    }
}
