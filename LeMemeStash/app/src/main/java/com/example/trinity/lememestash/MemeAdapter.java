package com.example.trinity.lememestash;

import android.content.Context;
import android.database.DataSetObserver;
import android.net.Uri;
import android.support.annotation.NonNull;
import android.view.View;
import android.view.ViewGroup;
import android.widget.ArrayAdapter;
import android.widget.BaseAdapter;
import android.widget.GridLayout;
import android.widget.ImageView;
import android.widget.LinearLayout;
import android.widget.ListAdapter;

import java.util.ArrayList;
import java.util.List;

/**
 * Created by Trinity on 1/31/2018.
 */

public class MemeAdapter extends ArrayAdapter<Meme> {

    public MemeAdapter(@NonNull Context context, int resource) {
        super(context, resource);
    }

    @Override
    public View getView(int i, View view, ViewGroup viewGroup) {
        Meme theMeme = this.getItem(i);

        ImageView returnView;
        if (view == null) {
            returnView = new ImageView(viewGroup.getContext());
            returnView.setAdjustViewBounds(true);
        } else {
            returnView = (ImageView)view;
        }
        returnView.setImageBitmap(theMeme.thumbnail);

        return returnView;
    }
}
